using BrokeProtocol.Utility.Networking;
using BrokeProtocolClient.settings;
using BrokeProtocolClient.utils;
using ENet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BrokeProtocolClient.modules.misc
{
    class ChatSpammer : Module
    {
        public ModeSetting mode = new ModeSetting("Mode", Mode.Input);
        public FileSetting file = new FileSetting("Messages file", FileManager.MainFolderPath + FileManager.MessagesFile);
        public NumberSetting delay = new NumberSetting("Delay (seconds)", 0.5, 10, 2.5, 0.5);

        public InputSetting message = new InputSetting("Message", 64, "BPclient on top!");
        public BooleanSetting randomize = new BooleanSetting("Add random text", false);
        public NumberSetting randomLength = new NumberSetting("Length of the random text", 1, 64, 5, 1);

        readonly static System.Random random = new System.Random();
        readonly string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        IEnumerator spammer;

        public ChatSpammer() : base(Categories.Misc, "Chat spammer", "Spams chat messages")
        {
            addSetting(mode);
            addSetting(file);
            addSetting(delay);

            addSetting(message);
            addSetting(randomize);
            addSetting(randomLength);
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
            if (!getClient().ClManager.myPlayer) setEnabled(false);

            while (true)
            {
                if (!mode.isMode((int)Mode.File))
                yield return new WaitForSeconds(delay.getValueFloat());

                if (!getClient().ClManager.myPlayer) setEnabled(false);

                if (mode.isMode((int)Mode.Input))
                {
                    // If randomize is enabled add random characters to the end of the message
                    string messege = randomize.isEnabled()
                        ? $"{message.getValue()} {new string(Enumerable.Repeat(chars, randomLength.getValueInt()).Select(s => s[random.Next(s.Length)]).ToArray())}"
                        : message.getValue();

                    PlayerUtils.SendMessage(messege);
                }

                else if (mode.isMode((int)Mode.File))
                {
                    string[] lines = File.ReadAllLines(FileManager.MainFolderPath + FileManager.MessagesFile);

                    foreach (string line in lines)
                    {
                        PlayerUtils.SendMessage(line);
                        yield return new WaitForSeconds(delay.getValueFloat());
                    }
                }

            }
        }



        enum Mode
        {
            Input,
            File
        }
    }
}
