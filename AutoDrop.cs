using BrokeProtocol.Client.UI;
using BrokeProtocol.Utility;
using BrokeProtocol.Utility.Networking;
using BrokeProtocolClient.settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrokeProtocolClient.modules.player
{
    class AutoDrop : Module
    {
        public ModeSetting dropMode = new ModeSetting("Drop mode", DropMode.DropAll);

        public InputSetting itemName = new InputSetting("Item name", 32, "Money");

        public BooleanSetting dropMax = new BooleanSetting("Drop all stack", true);
        public NumberSetting dropAmount = new NumberSetting("Drop amount", 1, 100, 1, 1);

        public AutoDrop() : base(Categories.Player, "Auto Drop", "Automatically drops items from your inventory")
        {
            addSetting(dropMode);
            addSetting(itemName);

            addSetting(dropMax);
            addSetting(dropAmount);
        }

        public override void onActivate()
        {
            getClient().ClManager.SendToServer((ENet.PacketFlags)1, SvPacket.Drop, Array.Empty<object>());
        }

        public override void onDeactivate()
        {
            ViewMenu menu = getClient().ClManager.CurrentMenu as ViewMenu;
            if (!menu) return;

            menu.CloseMenu(true);
        }

        public override void onRender()
        {
            
        }

        public override void onUpdate()
        {
            ViewMenu menu = getClient().ClManager.CurrentMenu as ViewMenu;
            if (!menu) return;

            if (dropMode.isMode((int)DropMode.DropAll))
            {
                foreach (KeyValuePair<int, InventoryItem> item in getClient().ClManager.myPlayer.myItems)
                {
                    menu.GiveAmount(item.Key, dropMax.isEnabled() ? item.Value.count : dropAmount.getValueInt());
                }
            }

            else if (dropMode.isMode((int)DropMode.DropItem))
            {
                foreach (KeyValuePair<int, InventoryItem> item in getClient().ClManager.myPlayer.myItems)
                {
                    if (item.Value.item.itemName != itemName.getValue()) continue;

                    menu.GiveAmount(item.Key, dropMax.isEnabled() ? item.Value.count : dropAmount.getValueInt());
                }
            }

            menu.CloseMenu(true);
        }

        enum DropMode
        {
            DropAll,
            DropItem
        }

    }
}
