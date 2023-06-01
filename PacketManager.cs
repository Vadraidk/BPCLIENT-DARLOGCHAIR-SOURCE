using BrokeProtocol.Collections;
using BrokeProtocol.Entities;
using BrokeProtocol.Utility.Networking;
using BrokeProtocolClient.settings;
using BrokeProtocolClient.utils;
using ENet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrokeProtocolClient.modules.misc
{
    class PacketManager : Module
    {
        BooleanSetting log = new BooleanSetting("Log Packets", true);
        BooleanSetting logUpdatePlayer = new BooleanSetting("Log UpdatePlayer", false);
        BooleanSetting logUpdateInput = new BooleanSetting("Log UpdateInput", false);

        BooleanSetting intercept = new BooleanSetting("Intercept Packets", false);

        InputSetting channelInput = new InputSetting("Channel", 4, "1337");
        InputSetting packetInput = new InputSetting("Packet", int.MaxValue, "");
        InputSetting argsInput = new InputSetting("Args (devide with \",\")", int.MaxValue, "");

        ActionSetting sendPacket;

        ActionSetting getEggs;

        public PacketManager() : base(Categories.Misc, "Packet Manager", "Allow to manage packets sent to server")
        {
            addSetting(log);
            addSetting(logUpdatePlayer);
            addSetting(logUpdateInput);

            addSetting(intercept);

            addSetting(channelInput);
            addSetting(packetInput);
            addSetting(argsInput);

            sendPacket = new ActionSetting("Send Packet", SendPacket);
            addSetting(sendPacket);

            getEggs = new ActionSetting("Get all eggs", GetAllEggs);
            addSetting(getEggs);
        }

        public void LogPacket(PacketFlags channel, SvPacket packet, params object[] args)
        {
            if (packet == SvPacket.UpdatePlayer && !logUpdatePlayer.isEnabled()) return;
            if (packet == SvPacket.UpdateInput && !logUpdateInput.isEnabled()) return;

            string arg = "";
            foreach (object item in args)
            {
                arg += $"{item.GetType()} {item} ";

                if (item is byte[])
                {
                    foreach (var bt in item as byte[])
                    {
                        arg += $"{bt}, ";
                    }
                }
                arg += "\n";
            }

            ConsoleBase.WriteLine($"{(PacketFlags)channel} Packet: {(SvPacket)packet}\n{arg}");
        }

        private void SendPacket()
        {
            PacketFlags channel = (PacketFlags)Enum.Parse(typeof(PacketFlags), channelInput.getValue());
            SvPacket packet = (SvPacket)Enum.Parse(typeof(SvPacket), packetInput.getValue());

            string[] stringArgs = argsInput.getValue().Trim().Split(',');
            object[] args = new object[stringArgs.Length];

            for (int i = 0; i < stringArgs.Length; i++)
            {
                string str = stringArgs[i];

                Int32 number;
                if (Int32.TryParse(str, out number))
                    args[i] = number;
                else
                    args[i] = str;

            }

            Client.instance.ClManager.SendToServer(channel, packet, args);
            Log($"Packet {channel} {packet} Sent!");
        }

        private void GetAllEggs()
        {
            // This was a quick exploit for an event on some server

            foreach (ShEntity entity in EntityCollections.Entities)
            {
                if (!entity.name.Contains("Jajco")) continue;

                getClient().ClManager.SendToServer(PacketFlags.Reliable, SvPacket.EntityAction, new object[]
                {
                    entity.ID,
                    "Get"
                });
            }
        }

        public bool shouldLogPacket()
        {
            return log.isEnabled();
        }

        public bool shouldInterceptPacket()
        {
            return intercept.isEnabled();
        }
    }
}
