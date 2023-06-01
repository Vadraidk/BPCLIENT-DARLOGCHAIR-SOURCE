using BrokeProtocol.Entities;
using BrokeProtocolClient.settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrokeProtocolClient.modules.movement
{
    class VechicleControll : Module
    {
        public NumberSetting speed = new NumberSetting("Speed", 0, 5, 2.5, 0.1);

        public VechicleControll() : base(Categories.Movement, "Vechicle Control", "Allows a better controll of vechicles")
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

            ShMovable vehicle = getClient().ClManager.myPlayer.curMount as ShMovable;
            if (!vehicle) return;
        }
    }
}
