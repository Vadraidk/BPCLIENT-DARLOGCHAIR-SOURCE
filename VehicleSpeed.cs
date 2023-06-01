using BrokeProtocol.Entities;
using BrokeProtocol.Utility.Networking;
using BrokeProtocolClient.settings;
using BrokeProtocolClient.utils;
using ENet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BrokeProtocolClient.patches;

namespace BrokeProtocolClient.modules.exploit
{
    class VehicleSpeed : Module
    {
        public NumberSetting speed = new NumberSetting("Speed", 0, 700, 50, 5);

        ShMovable vehicle;

        public VehicleSpeed() : base(Categories.Player, "VehicleSpeed", "make vehicle fast")
        {
            addSetting(speed);
        }

        public override void onActivate()
        {

        }

        public override void onDeactivate()
        {

        }

        public override void onRender()
        {

        }

        public override void onUpdate()
        {
            if (!getClient().ClManager.myPlayer) return;

            vehicle = getClient().ClManager.myPlayer.curMount as ShMovable;
            if (!vehicle) return;

            vehicle.maxSpeed = speed.getValueFloat();
            vehicle.speedLimit = speed.getValueFloat();
        }
    }
}
