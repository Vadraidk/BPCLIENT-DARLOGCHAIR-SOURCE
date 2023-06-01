using System;
using System.Collections.Generic;
using BrokeProtocolClient.modules;
using BrokeProtocolClient.settings;
using BrokeProtocolClient.UI;
using BrokeProtocolClient.utils;

namespace BrokeProtocolClient.modules.movement
{
    class Speed : Module
    {
        public NumberSetting speed = new NumberSetting("Speed", 0, 5, 2.5, 0.1);

        float defaultMaxSpeed = 12f;

        public Speed() : base(Categories.Movement, "Speed", "Allows to modify players speed") 
        {
            addSetting(speed);
        }

        public override void onActivate()
        {

        }

        public override void onDeactivate()
        {
            if (!getClient().ClManager.myPlayer) return;

            getClient().ClManager.myPlayer.maxSpeed = defaultMaxSpeed;
        }

        public override void onRender()
        {
            
        }

        public override void onUpdate()
        {
            if (!getClient().ClManager.myPlayer) return;

            getClient().ClManager.myPlayer.maxSpeed = defaultMaxSpeed * speed.getValueFloat();
        }
    }
}
