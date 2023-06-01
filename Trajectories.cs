using BrokeProtocol.Entities;
using BrokeProtocolClient.settings;
using BrokeProtocolClient.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;

namespace BrokeProtocolClient.modules.render
{
    class Trajectories : Module
    {
        BooleanSetting weaponInfo = new BooleanSetting("Weapon info", false);
        BooleanSetting allowProjectile = new BooleanSetting("Projectile weapons", true);
        BooleanSetting allowBallistic = new BooleanSetting("Ballistic weapons", true);
        BooleanSetting allowHitscan = new BooleanSetting("Hitscan weapons", true);

        NumberSetting linePoints = new NumberSetting("Line points", 1, 100, 50, 10);
        NumberSetting timeBetweenPoints = new NumberSetting("Time between points", 0.01, 0.25, 0.1, 0.01);

        NumberSetting traceWidth = new NumberSetting("Trace width", 0, 10, 1, 0.1);
        ColorSetting traceColor = new ColorSetting("Trace color", Color.cyan);

        AccessTools.FieldRef<ShMountable, float> nextFireRef = AccessTools.FieldRefAccess<ShMountable, float>("nextFire");

        public Trajectories() : base(Categories.Render, "Trajectories", "Predicts trajectory of weapons")
        {
            addSetting(weaponInfo);
            addSetting(allowProjectile);
            addSetting(allowBallistic);
            addSetting(allowHitscan);

            addSetting(linePoints);
            addSetting(timeBetweenPoints);
            
            addSetting(traceWidth);
            addSetting(traceColor);

        }

        public override void onRender()
        {
            if (!getClient().ClManager.myPlayer) return;

            ShEquipable equiped = getClient().ClManager.myPlayer.curEquipable;
            if (!equiped) return;

            if (weaponInfo.isEnabled())
            {
                string display = $"{(equiped as ShUsable).useDelay} {equiped.GetType()} {equiped.name}";
                Render.DrawWorldString(equiped.GetPosition, display, Color.white, 16);
            }

            if (allowProjectile.isEnabled())
            if (equiped is ShProjectile)
            {
                DrawProjectile(equiped as ShProjectile);
                return;
            }

            if (allowBallistic.isEnabled())
            if (equiped is ShBallistic)
            {
                DrawBalistic(equiped as ShBallistic);
                return;
            }

            if (allowHitscan.isEnabled())
            if (equiped is ShGun)
            {
                DrawHitscan(equiped as ShGun);
                return;
            }

        }

        private void DrawProjectile(ShProjectile gun)
        {
            var local = getClient().ClManager.myPlayer;
            if (!local) return;

            ShThrown thrownProjectile;
            if (!getClient().SceneManager.TryGetEntity<ShThrown>(gun.WeaponSet.thrownName, out thrownProjectile)) return;

            if (thrownProjectile.IsGuided) return;

            var startPos = local.OriginOffset();
            var startVelocity = local.Velocity + local.GetRotationT.forward * thrownProjectile.spawnVelocity;

            Color lineColor = traceColor.getColor();
            lineColor.a = 0.5f;
            Color circleColor = traceColor.getColor();

            RaycastHit hit = new RaycastHit();
            bool didhit = false;
            for (float time = 0; time < linePoints.getValueFloat();)
            {
                var point = startPos + startVelocity * time;
                point.y = startPos.y + startVelocity.y * time + (Physics.gravity.y * time * time / 2);

                time += timeBetweenPoints.getValueFloat();

                var nextPoint = startPos + startVelocity * time;
                nextPoint.y = startPos.y + startVelocity.y * time + (Physics.gravity.y * time * time / 2);

                if (Physics.Linecast(point, nextPoint, out hit))
                {
                    didhit = true;
                    break;
                }

                Render.DrawWorldLine(point, nextPoint, lineColor, traceWidth.getValueFloat());
            }

            if (didhit)
                Render.DrawWorldCircle(hit.point, hit.normal, 1, 16, circleColor, traceWidth.getValueFloat());
        }

        private void DrawBalistic(ShBallistic gun)
        {
            var local = getClient().ClManager.myPlayer;
            if (!local) return;

            var distanceLeft = gun.Range;
            var fireOffset = gun.NextFireVector();
            var velocity = gun.hitscanVelocity + fireOffset * gun.muzzleVelocity;
            var gravity = 0.04f * Physics.gravity;
            velocity += 0.5f * gravity;

            var pos = local.FutureOrigin;

            var nextPos = pos + velocity * 0.04f + 0.02f * gravity;
            var direction = nextPos - pos;
            var distance = direction.magnitude;

            RaycastHit hit = new RaycastHit();
            bool didhit = Physics.Raycast(pos, direction, out hit, distance, 26373);

            pos = nextPos;

            if (!didhit)
                while (distanceLeft > 0f)
                {
                    nextPos = pos + velocity * 0.04f + 0.02f * gravity;
                    direction = nextPos - pos;
                    distance = direction.magnitude;

                    velocity += gravity;
                    distanceLeft -= distance;

                    if (Physics.Raycast(pos, direction, out hit, distance, 26373))
                    {
                        didhit = true;
                        Render.DrawWorldLine(pos, hit.point, traceColor.getColor(), traceWidth.getValueFloat());
                        break;
                    }
                    Render.DrawWorldLine(pos, nextPos, traceColor.getColor(), traceWidth.getValueFloat());
                    pos = nextPos;
                }

            if (didhit)
            {
                Render.DrawWorldCircle(hit.point, hit.normal, 0.2f, 16, traceColor.getColor(), traceWidth.getValueFloat());
            }
            
        }

        private void DrawHitscan(ShGun gun)
        {
            var local = getClient().ClManager.myPlayer;
            if (!local) return;

            if (gun.burstSize > 1) // check if gun fires multiple projectiles
            {
                foreach (var fireVector in gun.NextFireVectors())
                {
                    RaycastHit hit;
                    if (!Physics.Raycast(local.GetOrigin, fireVector, out hit, gun.Range)) continue;

                    Render.DrawWorldCircle(hit.point, hit.normal, 0.2f, 16, traceColor.getColor(), traceWidth.getValueFloat());
                }

            }
            else
            {
                RaycastHit hit;
                if (!Physics.Raycast(local.GetOrigin, gun.NextFireVector(), out hit, gun.Range)) return;

                Render.DrawWorldCircle(hit.point, hit.normal, 0.2f, 16, traceColor.getColor(), traceWidth.getValueFloat());
            }
        }

    }
}
