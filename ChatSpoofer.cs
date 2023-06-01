using BrokeProtocolClient.settings;
using BrokeProtocolClient.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrokeProtocolClient.modules.exploit
{
    class ChatSpoofer : Module
    {
        InputSetting dummyMessage = new InputSetting("Dummy message", 128, "gg");
        InputSetting message = new InputSetting("Message", 128, "Hello :)");
        InputSetting target = new InputSetting("Send as (id/username)", 64, "");

        ActionSetting sendAs;
        ActionSetting sendToConsole;
        

        public ChatSpoofer() : base(Categories.Exploit, "Chat Spoofer", "Allow to exploit game chat")
        {
            addSetting(dummyMessage);

            addSetting(message);
            addSetting(target);

            sendAs = new ActionSetting("Send as target", SendAs);
            addSetting(sendAs);

            sendToConsole = new ActionSetting("Send to server console", SendConsole);
            addSetting(sendToConsole);
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


        void SendAs()
        {
            string mess = $@"{dummyMessage.getValue()}
{target.getValue()}: {message.getValue()}";

            PlayerUtils.SendMessage(mess);
        }

        void SendConsole()
        {
            string mess = $@"/
{message.getValue()}";

            PlayerUtils.SendMessage(mess);
        }

    }
}
