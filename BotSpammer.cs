using BrokeProtocolClient.settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BrokeProtocolClient.modules.exploit.botter
{
    class BotSpammer : Module
    {
        BotManager manager;

        ModeSetting mode;

        BooleanSetting randomServer = new BooleanSetting("Join random server", false);

        InputSetting hostName = new InputSetting("Host name", 15, "127.0.0.1");
        InputSetting port = new InputSetting("Port", 5, "5557");
        ActionSetting current;

        BooleanSetting randomName = new BooleanSetting("Random name (lenght of \"Name\")", true);
        InputSetting name = new InputSetting("Name", 16, "NotABot");

        NumberSetting messageAmount = new NumberSetting("Amount of messages", 1, 8, 8, 1);
        InputSetting message = new InputSetting("Message on join", 64, "BPclient on top!");

        ActionSetting start;
        ActionSetting remove;
        ActionSetting login;
        ActionSetting register;


        public BotSpammer() : base(Categories.Exploit, "Bot spammer", "Allows to spam bots")
        {
            manager = new BotManager();

            mode = new ModeSetting("Mode", BotManager.Mode.Auto);
            mode.SetOnChanged(() => manager.SetMode((BotManager.Mode)mode.getMode()));
            addSetting(mode);

            addSetting(randomServer);
            addSetting(hostName);
            addSetting(port);

            current = new ActionSetting("Set current server", Current);
            addSetting(current);

            addSetting(randomName);
            addSetting(name);

            start = new ActionSetting("Start", Start);
            addSetting(start);

            addSetting(messageAmount);
            addSetting(message);

            login = new ActionSetting("Login", Login);
            addSetting(login);

            register = new ActionSetting("Register", Register);
            addSetting(register);

            remove = new ActionSetting("Remove bot", Remove);
            addSetting(remove);

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

            Bot bot = new Bot(username, password, 0, 0, deviceID, "");

            bot.addOnJoinAction(
                new Action(() =>
                {
                    Client.instance.StartCoroutine(SpamThread(bot));
                }
            ));

            if (randomServer.isEnabled())
            {
                manager.ConnectRandomServer(bot);
            }
            else
            {
                manager.AddBot(bot, hostName.getValue(), ushort.Parse(port.getValue()));
            }
        }

        private void Current()
        {
            bool inGame = getClient().ClManager != null;

            hostName.setValue(inGame ? getClient().ClManager.connection.IP : "");
            port.setValue(inGame ? getClient().ClManager.connection.Port.ToString() : "");
        }

        private void Login()
        {
            manager.LoginAll();
        }

        private void Register()
        {
            manager.RegisterAll();
        }

        private void Remove()
        {
            manager.DisconnectAll();
        }

        private IEnumerator SpamThread(Bot bot)
        {
            for (int i = 0; i < messageAmount.getValueInt(); i++)
            {
                bot.SendGlobalMessage(message.getValue());
            }
            yield return new WaitForSeconds(0.5f);
            manager.DisconnectByName(bot.username);
        }

    }
}
