using BrokeProtocol.Entities;
using BrokeProtocol.Utility;
using BrokeProtocol.Utility.Networking;
using BrokeProtocolClient.modules.exploit.botter;
using BrokeProtocolClient.settings;
using BrokeProtocolClient.utils;
using ENet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BrokeProtocolClient.modules.exploit
{
    class Botter : Module
    {
        BotManager manager;
        ModeSetting mode;

        InputSetting hostName = new InputSetting("Host name", 15, "127.0.0.1");
        InputSetting port = new InputSetting("Port", 5, "5557");
        ActionSetting current;

        InputSetting username = new InputSetting("Username", 16, "");
        InputSetting password = new InputSetting("Password", 16, "");
        InputSetting deviceID = new InputSetting("Device ID", SystemInfo.deviceUniqueIdentifier.Length, "");
        InputSetting languageIndex = new InputSetting($"Language ID (0-{Util.languages.Length})", Util.languages.Length.ToString().Length, "0");
        ActionSetting create;

        BooleanSetting selectAll = new BooleanSetting("Select all", false);
        InputSetting selectedBot = new InputSetting("Selected bot (username)", 16, "");

        InputSetting chatMessage = new InputSetting("Chat", 64, "BPclient on top!");
        ActionSetting send;

        ActionSetting remove;

        ActionSetting login;
        ActionSetting register;


        public Botter() : base(Categories.Exploit, "Botter", "Allows to create and manage bots")
        {
            manager = new BotManager();

            mode = new ModeSetting("Mode", BotManager.Mode.Auto);
            mode.SetOnChanged(() => manager.SetMode((BotManager.Mode)mode.getMode()));
            addSetting(mode);

            addSetting(hostName);
            addSetting(port);

            current = new ActionSetting("Set current server", Current);
            addSetting(current);

            addSetting(username);
            addSetting(password);
            addSetting(deviceID);

            create = new ActionSetting("Create bot", Create);
            addSetting(create);

            addSetting(selectAll);
            addSetting(selectedBot);
            addSetting(chatMessage);

            send = new ActionSetting("Send chat message", Send);
            addSetting(send);

            remove = new ActionSetting("Remove bot", Remove);
            addSetting(remove);

            addSetting(new InfoSetting("Manual:"));
            login = new ActionSetting("Login", Login);
            addSetting(login);

            register = new ActionSetting("Register", Register);
            addSetting(register);

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

        private void Current()
        {
            bool inGame = getClient().ClManager != null;

            hostName.setValue(inGame ? getClient().ClManager.connection.IP : "");
            port.setValue(inGame ? getClient().ClManager.connection.Port.ToString() : "");
        }

        private void Create()
        {
            Bot bot = new Bot(username.getValue(), password.getValue(), 1, Int32.Parse(languageIndex.getValue()), deviceID.getValue(), "");
            manager.AddBot(bot, hostName.getValue(), ushort.Parse(port.getValue()));
        }

        private void Remove()
        {
            if (selectAll.isEnabled()) manager.DisconnectAll();
            else manager.DisconnectByName(selectedBot.getValue());
        }

        private void Send()
        {
            if (selectAll.isEnabled()) manager.SendMessageAll(chatMessage.getValue());
            else manager.SendMessageByName(selectedBot.getValue(), chatMessage.getValue());
        }

        private void Login()
        {
            if (selectAll.isEnabled()) manager.LoginAll();
            else manager.Login(selectedBot.getValue());
        }

        private void Register()
        {
            if (selectAll.isEnabled()) manager.RegisterAll();
            else manager.Register(selectedBot.getValue());
        }

    }
}
