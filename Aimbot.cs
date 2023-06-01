using BrokeProtocol.Client.UI;
using BrokeProtocol.Collections;
using BrokeProtocol.Entities;
using BrokeProtocol.Utility;
using BrokeProtocolClient.modules.misc;
using BrokeProtocolClient.settings;
using BrokeProtocolClient.utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace BrokeProtocolClient.modules.combat
{
    class Aimbot : Module
    {
        ModeSetting mode = new ModeSetting("Mode", Mode.Silent);

        BooleanSetting onAiming = new BooleanSetting("Activate only when aiming", true);

        BooleanSetting humans = new BooleanSetting("Players", true);
        BooleanSetting npcs = new BooleanSetting("Npcs", false);

        BooleanSetting showFov = new BooleanSetting("Show Fov", true);
        BooleanSetting showTarget = new BooleanSetting("Show Target", true);
        NumberSetting fov = new NumberSetting("Fov", 0, 1000, 150, 5);

        BooleanSetting wallCheck = new BooleanSetting("Wall Check", true);

        NumberSetting smoothing = new NumberSetting("Legit Smoothing", 0, 30, 15, 0.5);

        ColorSetting fovCircleColor = new ColorSetting("Fov color", Color.cyan);
        NumberSetting alpha = new NumberSetting("No target alpha", 0, 1, 0.25, 0.05);

        Vector2 center = new Vector2(Screen.width / 2, Screen.height / 2);
        ShPlayer target;

        List<ShPlayer> entities = new List<ShPlayer>();

        public Quaternion originalRotation;

        public Aimbot() : base(Categories.Combat, "Aimbot", "Aims at enemies")
        {
            addSetting(mode);
            addSetting(onAiming);

            addSetting(humans);
            addSetting(npcs);

            addSetting(showFov);
            addSetting(showTarget);
            addSetting(fov);

            addSetting(wallCheck);
            addSetting(smoothing);

            addSetting(fovCircleColor);
            addSetting(alpha);
        }

        public override void onActivate()
        {
            target = getTarget();
        }

        public override void onDeactivate()
        {
            target = null;
        }

        public override void onRender()
        {
            if (showFov.isEnabled())
            {
                Color circleColor = fovCircleColor.getColor();

                circleColor.a = !target ? alpha.getValueFloat() : 1f;
                Render.DrawCircle(center, fov.getValueFloat(), 32, 2, circleColor);

                if (!target) return;
                if (!showTarget.isEnabled()) return;
                Vector2 headScreenPos = getClient().MainCamera.worldCamera.WorldToScreenPoint(target.headCollider.bounds.center);

                Render.DrawLine(center, new Vector2(headScreenPos.x, Screen.height - headScreenPos.y), getClient().Rainbow, 1);
            }
        }

        public override void onUpdate()
        {
            center = new Vector2(Screen.width / 2, Screen.height / 2);

            var local = getClient().ClManager.myPlayer;
            if (!local) return;

            target = null;
            if (!shouldAim()) return;

            target = getTarget();
            if (!target) return;

            if (shouldSilentAim()) return;

            Quaternion bestAimRotation = getAimRotation(target);

            local.SetRotation(bestAimRotation);
        }

        public Quaternion getAimRotation(ShPlayer target)
        {
            Vector3 bestAimPos = predictMovement(target);
            Quaternion bestAimRotation = Quaternion.LookRotation(bestAimPos);

            if (mode.isMode((int)Mode.Legit))
            {
                Quaternion nextRotation = Quaternion.Lerp(getClient().ClManager.myPlayer.GetRotation, bestAimRotation, smoothing.getValueFloat() * Time.deltaTime);
                bestAimRotation = nextRotation;
            }

            return bestAimRotation;
        }

        public ShPlayer getTarget()
        {
            ShPlayer target = null;
            float distance = Mathf.Infinity;

            entities.Clear();
            if (humans.isEnabled()) entities.AddRange((EntityCollections.Humans as Collection<ShPlayer>).ToList());
            if (npcs.isEnabled()) entities.AddRange((EntityCollections.NPCs as Collection<ShPlayer>).ToList());

            foreach (ShPlayer player in entities)
            {
                if (player.IsDead) continue;
                if (player == getClient().ClManager.myPlayer) continue;
                if (player.headCollider.bounds.size == Vector3.zero) continue;
                if (FriendManager.getFriends().Contains(player.username)) continue;

                RaycastHit raycastHit;
                if (wallCheck.isEnabled())
                if (Physics.Raycast(getClient().ClManager.myPlayer.originT.position, (player.originT.position - getClient().ClManager.myPlayer.originT.position).normalized, out raycastHit, 9985))
                {
                    ShPlayer hitPlayer = raycastHit.collider.GetComponentInParent<ShPlayer>();
                    if (!hitPlayer) continue;
                }

                Vector3 headScreenPos = MonoBehaviourSingleton<MainCamera>.Instance.worldCamera.WorldToScreenPoint(player.headCollider.bounds.center);

                if (headScreenPos.z < 0f) continue;

                var _distance = Vector3.Distance(center, headScreenPos);
                var _screenDistance = Vector2.Distance(center, headScreenPos);

                if (_distance < distance && _screenDistance < fov.getValue())
                {
                    distance = _distance;
                    target = player;
                }
            }

            return target;
        }

        private Vector3 predictMovement(ShPlayer target)
        {
            Vector3 pos = (target.headCollider.bounds.center - getClient().ClManager.myPlayer.headCollider.bounds.center);
            ShGun gun = getClient().ClManager.myPlayer.curEquipable as ShGun;

            if (gun)
            {
                float distance = Vector3.Distance(getClient().ClManager.myPlayer.headCollider.bounds.center, target.headCollider.bounds.center);
                float timeOffset = distance / gun.WeaponVelocity;
                float gravityOffset = (distance * timeOffset) / gun.WeaponGravity;
                pos += (target.relativeVelocity * timeOffset);
                pos -= (getClient().ClManager.myPlayer.relativeVelocity * timeOffset);
                pos -= (Vector3.up * gravityOffset);
            }

            return pos;
        }

        public bool shouldAim()
        {
            if (onAiming.isEnabled() && !getClient().ClManager.GetButton(InputType.Zoom, false)) return false;
            else return true;
        }

        public bool shouldSilentAim()
        {
            return mode.isMode((int)Mode.Silent);
        }

        public void PreSilent()
        {
            var local = getClient().ClManager.myPlayer;
            if (!local) return;

            if (!shouldAim() || !shouldSilentAim()) return;

            originalRotation = local.GetRotation;

            ShPlayer target = getTarget();
            if (!target) return;

            Quaternion rotation = getAimRotation(target);

            local.SetRotation(rotation);
        }

        public void PostSilent()
        {
            var local = getClient().ClManager.myPlayer;
            if (!local) return;

            if (!shouldAim() || !shouldSilentAim()) return;

            local.SetRotation(originalRotation);
        }

        enum Mode
        {
            Rage,
            Silent,
            Legit
        }

    }
}
