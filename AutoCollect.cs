using BrokeProtocol.Collections;
using BrokeProtocol.Entities;
using BrokeProtocol.Utility.Networking;
using BrokeProtocolClient.settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrokeProtocolClient.modules.player
{
    class AutoCollect : Module
    {
        public NumberSetting range = new NumberSetting("Range", 1, 16, 5, 0.5);

        public AutoCollect() : base(Categories.Player, "Auto Collect", "Automatically collects items around you")
        {
            addSetting(range);
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

            foreach (ShEntity item in EntityCollections.Entities)
            {
                if (getClient().ClManager.myPlayer.Distance(item) > range.getValueFloat()) continue;
                if (!getClient().ClManager.myPlayer.CanCollectEntity(item)) continue;

                if ((item as ShExplosion).armed) continue;

                getClient().ClManager.SendToServer((ENet.PacketFlags)1, SvPacket.Collect, new object[]
                {
                    item.ID,
                    false
                });
            }
        }
    }
}
