using BrokeProtocol.Utility.Networking;
using BrokeProtocolClient.settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrokeProtocolClient.modules.player
{
    class NoRestrain : Module
    {
        public BooleanSetting onHit = new BooleanSetting("Unrestrain on hit", true);

        public NoRestrain() : base(Categories.Player, "No Restrain", "Automatically unrestrains yourself")
        {
            addSetting(onHit);
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
            if (!getClient().ClManager.myPlayer.IsRestrained) return;

            if (onHit.isEnabled())
            {
                if (!getClient().ClManager.fire) return;
            }

            getClient().ClManager.SendToServer(ENet.PacketFlags.Reliable, SvPacket.Free, new object[]
            {
                getClient().ClManager.myPlayer.ID
            });
        }
    }
}
