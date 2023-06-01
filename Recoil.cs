using BrokeProtocol.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrokeProtocolClient.modules.combat
{
    class Recoil : Module
    {
        public Recoil() : base(Categories.Combat, "Recoil", "Modifies gun recoil")
        {

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
            ShGun gun = getClient().ClManager.myPlayer.curEquipable as ShGun;
            if (gun)
            {
                gun.recoilFactor = 0;
            }
        }
    }
}
