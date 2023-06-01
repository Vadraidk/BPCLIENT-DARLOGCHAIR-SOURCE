using BrokeProtocol.Entities;
using BrokeProtocol.Required;
using BrokeProtocol.Utility.Networking;
using BrokeProtocolClient.utils;
using ENet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrokeProtocolClient.modules.exploit
{
    class AutoStand : Module
    {
        public AutoStand() : base(Categories.Exploit, "Auto Stand", "")
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
            ShPlayer local = getClient().ClManager.myPlayer;
            if (!local) return;

            StanceIndex stance = local.GetStanceIndex;

            if (stance != StanceIndex.KnockedDown &&
                stance != StanceIndex.KnockedOut &&
                stance != StanceIndex.Recovering) 
                return;

            getClient().ClManager.SendToServer(PacketFlags.Reliable, SvPacket.Crouch, new object[]
            {
                false
            });
            local.SetStance(StanceIndex.Stand);
            ConsoleBase.WriteLine("Crouch");

        }
    }
}
