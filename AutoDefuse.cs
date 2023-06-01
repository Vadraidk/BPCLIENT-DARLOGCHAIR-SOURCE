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
    class AutoDefuse : Module
    {
        public NumberSetting range = new NumberSetting("Range", 1, 16, 5, 0.5);

        public AutoDefuse() : base(Categories.Player, "Auto Defuse", "Automatically defuses mines around you")
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

            foreach (ShEntity entity in EntityCollections.Entities)
            {
                if (getClient().ClManager.myPlayer.Distance(entity) > range.getValueFloat()) continue;

                ShExplosion explosion = entity as ShExplosion;
                if (!explosion) continue;
                if (!explosion.isWorldEntity || !explosion.armed) continue;

                getClient().ClManager.SendToServer((ENet.PacketFlags)1, SvPacket.Disarm, new object[]
                {
                    entity.ID
                });
            }
        }
    }
}
