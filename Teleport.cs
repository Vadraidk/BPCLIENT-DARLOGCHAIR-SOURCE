using BrokeProtocol.Collections;
using BrokeProtocol.Entities;
using BrokeProtocol.Utility.Networking;
using BrokeProtocolClient.settings;
using BrokeProtocolClient.utils;
using ENet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace BrokeProtocolClient.modules.exploit
{
    class Teleport : Module
    {
        ModeSetting mode = new ModeSetting("Mode", Mode.Crosshair);
        BindSetting teleportBind = new BindSetting(0, "Quick teleport bind");

        InputSetting targetInput = new InputSetting("Target Username/ID", 16, "");

        ActionSetting teleport;

        Vector3 teleportPos;

        public Teleport() : base(Categories.Exploit, "Teleport", "Allows to teleport")
        {
            addSetting(mode);

            addSetting(new InfoSetting("Teleport bind:"));
            addSetting(teleportBind);

            addSetting(targetInput);

            teleport = new ActionSetting("Teleport", TeleportToPlayer);
            addSetting(teleport);


        }

        public override void onActivate()
        {
            
        }

        public override void onDeactivate()
        {
            
        }

        public override void onRender()
        {
            if (mode.isMode(Mode.Crosshair))
            {
                if (teleportPos == Vector3.zero) return;

                ShPlayer local = getClient().ClManager.myPlayer;
                if (!local) return;

                Color circleColor = new Color(0, 1, 0, 0.5f);
                Color lineColor = new Color(0, 1, 1, 0.25f);

                Render.DrawWorldCircle(teleportPos, 1.5f, 16, circleColor, 2);
                Render.DrawWorldLine(local.GetPosition, teleportPos, lineColor, 2);
            }
        }

        public override void onUpdate()
        {
            ShPlayer local = getClient().ClManager.myPlayer;
            if (!local) return;

            Camera camera = getClient().MainCamera.worldCamera;
            if (!camera) return;

            if (mode.isMode(Mode.Crosshair))
            {
                RaycastHit raycastHit;
                if (!Physics.Raycast(camera.transform.position, camera.transform.forward, out raycastHit, 9985))
                {
                    teleportPos = Vector3.zero;
                    return;
                }

                teleportPos = raycastHit.point;

                if (teleportBind.WasPressedThisFrame())
                    TeleportTo(teleportPos);
            }
        }

        private void TeleportToPlayer()
        {
            ShPlayer target;
            if (!EntityCollections.TryGetPlayerByNameOrID(targetInput.getValue(), out target))
            {
                Log($"{targetInput.getValue()} not found!");
                return;
            }

            if (target.headCollider.bounds.size == Vector3.zero)
            {
                Log($"{targetInput.getValue()} is too far away!");
                return;
            }

            TeleportTo(target.GetPosition);
        }

        private void TeleportTo(Vector3 pos)
        {
            ShPlayer local = getClient().ClManager.myPlayer;
            if (!local) return;

            float distance = local.DistanceSqr(pos);
            int jumps = 1;
            float velocity = 0;

            do
            {
                jumps++;
                velocity += 14f;
            } while (distance > velocity * velocity * 0.25f + 1.5f);    // From SvTryUpdateSmooth in Scripts.dll

            for (int i = 0; i < jumps; i++)
            {
                getClient().ClManager.SendToServer(PacketFlags.Reliable, SvPacket.Jump, new object[] { });
            }

            getClient().ClManager.myPlayer.SetPosition(pos);
        }

        enum Mode
        {
            Player,
            Crosshair
        }

    }
}
