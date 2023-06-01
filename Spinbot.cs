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
    class Spinbot : Module
    {

        public NumberSetting rotationSpeed = new NumberSetting("Rotation speed", -360, 360, 0, 1);
        public BooleanSetting lookDown = new BooleanSetting("Look down", false);


        Quaternion rotation;

        public Spinbot() : base(Categories.Combat, "Spinbot", "Spins your player")
        {
            addSetting(rotationSpeed);
            addSetting(lookDown);
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

            eulerAngles.y += rotationSpeed.getValueInt() * Time.deltaTime;

            Log($"Rotation speed: {rotationSpeed.getValueInt() * Time.deltaTime}");

            if (lookDown.isEnabled())
                eulerAngles.x = 90.1f;
            else
                eulerAngles.x = getClient().ClManager.myPlayer.GetRotation.eulerAngles.x;

            rotation = Quaternion.Euler(eulerAngles);
        }

        public override bool onSendToServer(PacketFlags channel, SvPacket packet, params object[] args)
        {
            

            return true;
        }

        public bool ShouldIgnoreRotationUpdate()
        {
            return true;
        }
    }
}
