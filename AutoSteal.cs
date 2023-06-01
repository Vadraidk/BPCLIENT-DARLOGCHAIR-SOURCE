using BrokeProtocol.Client.UI;
using BrokeProtocol.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrokeProtocolClient.modules.player
{
    class AutoSteal : Module
    {


        public AutoSteal() : base(Categories.Player, "Auto Steal", "Automatically steals items targets inventory")
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
            ViewMenu menu = getClient().ClManager.CurrentMenu as ViewMenu;
            if (!menu) return;

            foreach (KeyValuePair<int, InventoryItem> item in getClient().ClManager.myPlayer.otherEntity.myItems)
            {
                menu.TakeAmount(item.Key, item.Value.count);
            }

        }
    }
}
