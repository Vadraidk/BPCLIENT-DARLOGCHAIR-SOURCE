using BrokeProtocol.Utility.Networking;
using BrokeProtocolClient.settings;
using BrokeProtocolClient.utils;
using ENet;
using System.Collections;
using UnityEngine;
using BrokeProtocol.Entities;
using BrokeProtocol.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BrokeProtocolClient.modules.misc
{
    class TradeSpammer : Module
    {
        ModeSetting mode = new ModeSetting("Mode", Mode.Target);
        BooleanSetting allowPlayers = new BooleanSetting("Players", true);
        BooleanSetting allowNpcs = new BooleanSetting("Npcs", false);

        InputSetting targetInput = new InputSetting("Targets username/ID", 16, "");

        NumberSetting delay = new NumberSetting("Delay (seconds)", 0, 10, 0.1, 0.1);

        IEnumerator spammer;

        public TradeSpammer() : base(Categories.Misc, "Trade spammer", "Spams trade requests")
        {
            addSetting(mode);
            addSetting(allowPlayers);
            addSetting(allowNpcs);

            addSetting(targetInput);
            addSetting(delay);
        }

        public override void onActivate()
        {
            if (spammer != null) return;

            spammer = SpammerThread();
            getClient().StartCoroutine(spammer);
        }

        public override void onDeactivate()
        {
            if (spammer == null) return;

            getClient().StopCoroutine(spammer);
            spammer = null;
        }

        public override void onRender()
        {

        }

        public override void onUpdate()
        {

        }

        private IEnumerator SpammerThread()
        {
            while (true)
            {
                yield return new WaitForSeconds(delay.getValueFloat());

                if (!getClient().ClManager.myPlayer) break;

                if (mode.isMode((int)Mode.Target))
                {
                    ShPlayer target;
                    if (!EntityCollections.TryGetPlayerByNameOrID(targetInput.getValue(), out target))
                    {
                        ConsoleBase.WriteLine($"{targetInput.getValue()} not found!");
                        break;
                    }

                    if (!allowPlayers.isEnabled() && target.isHuman) break;
                    if (!allowNpcs.isEnabled() && !target.isHuman) break;

                    SendTradeRequest(target);

                }
                else if (mode.isMode((int)Mode.All))
                {
                    List<ShPlayer> targets = new List<ShPlayer>();
                    if (allowPlayers.isEnabled()) targets.AddRange((EntityCollections.Humans as Collection<ShPlayer>).ToList());
                    if (allowNpcs.isEnabled()) targets.AddRange((EntityCollections.NPCs as Collection<ShPlayer>).ToList());

                    foreach (ShPlayer target in targets)
                    {
                        if (target == getClient().ClManager.myPlayer) continue;

                        SendTradeRequest(target);
                    }
                }

            }

            setEnabled(false);
        }

        private void SendTradeRequest(ShPlayer player)
        {
            if (!player.isWorldEntity || player.IsDead || !player.IsMobile || player.Shop) return;

            getClient().ClManager.SendToServer((PacketFlags)1, SvPacket.TradeRequest, new object[]
            {
                player.ID
            });
        }

        enum Mode
        {
            Target,
            All
        }

    }
}
