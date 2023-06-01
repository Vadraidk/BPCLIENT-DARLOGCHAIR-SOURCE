using BrokeProtocol.Client.UI;
using BrokeProtocol.Collections;
using BrokeProtocol.Entities;
using BrokeProtocol.Managers;
using BrokeProtocol.Required;
using BrokeProtocol.Utility;
using BrokeProtocol.Utility.Networking;
using BrokeProtocolClient.modules.exploit.botter;
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
using UnityEngine.UIElements;

namespace BrokeProtocolClient.modules.exploit.botter
{
    class AutoBotTrade : Module
    {
        BotManager botManager;

        BooleanSetting randomName = new BooleanSetting("Random name (lenght of \"Name\")", true);
        InputSetting name = new InputSetting("Name", 16, "NotABot");

        ActionSetting start;

        Bot bot;

        public AutoBotTrade() : base(Categories.Exploit, "Auto bot trade", "Allows to farm items by creating new bots")
        {
            addSetting(randomName);
            addSetting(name);

            start = new ActionSetting("Start", Start);
            addSetting(start);

            botManager = new BotManager();
            botManager.SetMode(BotManager.Mode.Auto);
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
            
        }

        public void Start()
        {
            string username = randomName.isEnabled() ? utils.Utils.RandomString(name.getValue().Length) : name.getValue();
            string password = utils.Utils.RandomString(16);
            string deviceID = utils.Utils.RandomString(SystemInfo.deviceUniqueIdentifier.Length);

            bot = new Bot(username, password, 0, 0, deviceID, "");

            // Make the bot trade with your player on join
            bot.addOnJoinAction(
                new Action(() => bot.SendTradeRequest(getClient().ClManager.myPlayer.ID.ToString())
            ));

            botManager.AddBot(bot, getClient().ClManager.connection.IP, getClient().ClManager.connection.Port);

            ShPlayer target;

            if (EntityCollections.TryGetPlayerByNameOrID(bot.username, out target))
            {
                PlayerUtils.SendTradeRequest(target.ID);



                Client.instance.StartCoroutine(Trade());
            }
            else
            {
                ConsoleBase.WriteLine("not found !");
            }
        }

        public void HandleGameMessage(string message)
        {
            if (String.IsNullOrWhiteSpace(message)) return;

            int location = message.IndexOf(" wants to trade");
            if (location <= 0) return;

            string playerName = message.Substring(0, location);

            ShPlayer target;
            if (!EntityCollections.TryGetPlayerByNameOrID(playerName, out target))
            {
                ConsoleBase.WriteLine($"{target.username} not found!");
                return;
            }


        }



        private IEnumerator Trade()
        {
            yield return new WaitForSeconds(10f);

            string searchedName = "Money";

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
            }

            bot.SendToServer(ENet.PacketFlags.Reliable, BrokeProtocol.Utility.Networking.SvPacket.TransferTrade, new object[]
            {
                getClient().ClManager.myPlayer,
                (byte)6,
                item.index,
                1000
            });

            bot.SendToServer(ENet.PacketFlags.Reliable, BrokeProtocol.Utility.Networking.SvPacket.ConfirmTrade, Array.Empty<object>());
            getClient().ClManager.SendToServer(ENet.PacketFlags.Reliable, BrokeProtocol.Utility.Networking.SvPacket.ConfirmTrade, Array.Empty<object>());
        }
    }
}
