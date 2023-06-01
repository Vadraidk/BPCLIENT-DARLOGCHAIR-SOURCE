using BrokeProtocol.Utility.Networking;
using BrokeProtocolClient.settings;
using ENet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BrokeProtocolClient.modules.combat
{
    class Antiaim : Module
    {
        NumberSetting rotationX = new NumberSetting("Rotation x", -1, 1, 0, 0.01);
        NumberSetting rotationY = new NumberSetting("Rotation y", -1, 1, 0, 0.01);
        NumberSetting rotationZ = new NumberSetting("Rotation z", -1, 1, 0, 0.01);

        BooleanSetting logRotation = new BooleanSetting("Log rotation", false);

        Quaternion rotation;

        public Antiaim() : base(Categories.Combat, "Antiaim", "Makes it harder to shoot you")
        {
            addSetting(rotationX);
            addSetting(rotationY);
            addSetting(rotationZ);

            addSetting(logRotation);
        }

        public override void onActivate()
        {
            if (!getClient().ClManager.myPlayer) return;
            rotation = getClient().ClManager.myPlayer.GetRotation;
        }

        public override void onUpdate()
        {
            if (!getClient().ClManager.myPlayer) return;

            Vector3 eulerAngles = rotation.eulerAngles;

            if (logRotation.isEnabled())
                Log($"{eulerAngles.x} | {eulerAngles.y} | {eulerAngles.z}");

            eulerAngles.y = eulerAngles.y - 180f;


            rotation = Quaternion.Euler(eulerAngles);

            getClient().ClManager.SendToServer(PacketFlags.Reliable, SvPacket.UpdateRotation, new object[]
            {
                rotation
            });
        }

        public override bool onSendToServer(PacketFlags channel, SvPacket packet, params object[] args)
        {
            

            return true;
        }
    }
}
