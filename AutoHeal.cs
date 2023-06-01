using BrokeProtocol.Entities;
using BrokeProtocol.Utility;
using BrokeProtocol.Utility.Networking;
using BrokeProtocolClient.settings;
using BrokeProtocolClient.utils;
using ENet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrokeProtocolClient.modules.player
{
    class AutoHeal : Module
    {
        public NumberSetting minHealth = new NumberSetting("Heal below", 50, 180, 90, 5);
        public BooleanSetting whenInjured = new BooleanSetting("Only when injured", false);

        float lastHealth;

        public AutoHeal() : base(Categories.Player, "Auto Heal", "Automatically consumes healing items from your inventory")
        {
            addSetting(minHealth);
            addSetting(whenInjured);
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

            if (lastHealth <= getClient().ClManager.myPlayer.health) return;

            if (whenInjured.isEnabled() && !getClient().ClManager.myPlayer.injuries.Any()) return;

            while (getClient().ClManager.myPlayer.health < minHealth.getValueFloat())
            {
                foreach (KeyValuePair<int, InventoryItem> itemSlot in getClient().ClManager.myPlayer.myItems)
                {
                    
                    if ((itemSlot.Value.item as ShConsumable).healthBoost <= 0) continue;

                    getClient().ClManager.SendToServer(PacketFlags.Reliable, SvPacket.Consume, new object[]
                    {
                        itemSlot.Key
                    });
                    break;
                }
            }

            lastHealth = getClient().ClManager.myPlayer.health;
        }

    }
}
