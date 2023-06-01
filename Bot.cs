using BrokeProtocol.Collections;
using BrokeProtocol.Entities;
using BrokeProtocol.Managers;
using BrokeProtocol.Required;
using BrokeProtocol.Utility;
using BrokeProtocol.Utility.Networking;
using BrokeProtocolClient.modules.misc;
using BrokeProtocolClient.utils;
using ENet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using EventType = ENet.EventType;

namespace BrokeProtocolClient.modules.exploit.botter
{
    class Bot
    {
        public BotManager manager;

        Host client;
        Peer connection;

        List<Action> onJoinActions = new List<Action>();

        IEnumerator connectionThread;
        IEnumerator playerUpdateThread;

        private bool joined = false;

        public string username;
        public string password;
        public byte skinIndex;
        public int languageIndex;
        public string deviceID;
        public string profileURL;

        public Bot(string username, string password, byte skinIndex, int languageIndex, string deviceID, string profileURL)
        {
            this.username = username;
            this.password = password;
            this.skinIndex = skinIndex;
            this.languageIndex = languageIndex;
            this.deviceID = deviceID;
            this.profileURL = profileURL;
        }

        public void Connect(string hostip, ushort port)
        {
            this.client = new Host();
            Address address = new Address();

            address.SetHost(hostip);
            address.Port = port;
            client.Create();

            this.connection = client.Connect(address, 0, (uint)Client.instance.ShManager.Version.GetPrefabIndex());

            if (connectionThread != null)
            {
                Client.instance.StopCoroutine(connectionThread);
                connectionThread = null;
            }

            connectionThread = ConnectionLoop();
            Client.instance.StartCoroutine(connectionThread);
        }

        private void onConnect()
        {
            connection.Timeout(32U, 5000U, 30000U);
            connection.ConfigureThrottle(5000U, 2U, 2U, 40U);

            SendToServer(PacketFlags.Reliable, SvPacket.Ready, new object[]{ Application.platform });
        }

        public void SendGlobalMessage(string message)
        {
            SendToServer(PacketFlags.Reliable, SvPacket.ChatGlobal, new object[]{ message });
        }

        public void SendLocalMessage(string message)
        {
            SendToServer(PacketFlags.Reliable, SvPacket.ChatLocal, new object[]{ message });
        }

        public void SendTradeRequest(string targetUsernameOrID)
        {
            ShPlayer target;
            if (EntityCollections.TryGetPlayerByNameOrID(targetUsernameOrID, out target))
            {
                if (target.isWorldEntity || !target.IsDead || target.IsMobile || !target.Shop)
                {
                    SendToServer((PacketFlags)1, SvPacket.TradeRequest, new object[]
                    {
                        target.ID
                    });
                    return;
                }

            }
            else
                ConsoleBase.WriteLine($"{targetUsernameOrID} not found!");

            int ID;
            if (int.TryParse(targetUsernameOrID, out ID))
            SendToServer((PacketFlags)1, SvPacket.TradeRequest, new object[]
            {
                ID
            });

        }

        private void handlePacket(ClPacket packetID)
        {
            switch (packetID.ToString())
            {
                case "TransferInfo":
                    {
                        Dictionary<string, AssetData> assetFiles = new Dictionary<string, AssetData>();
                        string mapHash;

                        int amount = Buffers.reader.ReadInt32();

                        for (int i = 0; i < amount; i++)
                        {
                            assetFiles[Buffers.reader.ReadString()] = null;
                        }
                        mapHash = Buffers.reader.ReadString();

                        byte[] assetCacheExists = new byte[assetFiles.Count];
                        for (int i = 0; i < assetCacheExists.Length; i++)
                        {
                            assetCacheExists[i] = 1;
                        }


                        Log($"Sending {packetID} response...");
                        SendToServer(PacketFlags.Reliable, SvPacket.Cache, new object[]
                        {
                            assetCacheExists,
                            true
                        });
                        break;
                    }

                case "FinishedStaticData":
                    {
                        Log($"Sending {packetID} response...");
                        SendToServer(PacketFlags.Reliable, SvPacket.Loaded, new object[]{ });
                        break;
                    }

                case "RegisterMenu":
                    {
                        List<string> list = new List<string>();

                        bool canLogin = Buffers.reader.ReadBoolean();

                        int amount = Buffers.reader.ReadInt32();
                        for (int i = 0; i < amount; i++)
                        {
                            list.Add(Buffers.reader.ReadString());
                        }

                        if (manager.IsMode(BotManager.Mode.Manual))
                        {
                            AskForInput();
                            break;
                        }

                        string registerType = canLogin ? "Login" : "Register";
                        Log($"Sending {packetID} type: {registerType} response...");

                        if (canLogin) Login();
                        else Register();
                        break;
                    }

                case "RegisterFail":
                    {
                        string message = Buffers.reader.ReadString();
                        Log($"Register fail: {message}");

                        if (manager.IsMode(BotManager.Mode.Auto))
                        {
                            Log("Trying again...");
                            if (message.Contains("Account not found"))
                            {
                                Log($"Trying to register...");
                                Register();
                            }

                            else if (message.Contains("Invalid credentials"))
                            {
                                Log($"Account already exists, wrong password!");
                            }

                            else if (message.Contains("Invalid data"))
                            {
                                Log($"Invalid data in skinIndex - {skinIndex} or wearableIndices!");
                            }
                        }
                        else if (manager.IsMode(BotManager.Mode.Manual))
                        {
                            AskForInput();
                        }

                        break;
                    }

                case "UpdateSmooth":
                    // Run only on first update packet
                    if (!joined)
                    {
                        joined = true;

                        foreach (Action onJoinAction in onJoinActions)
                        {
                            onJoinAction.Invoke();
                        }
                    }
                    break;

                case "ChatGlobal":
                    {
                        int senderID = Buffers.reader.ReadInt32();
                        string message = Buffers.reader.ReadString();

                        Log($"{senderID} : {message}");

                        if (!message.StartsWith(".")) break;
                        if (message.ToLower().StartsWith(".help"))
                        {
                            SendGlobalMessage($"List of commands: .help, .hello");
                        }
                        else if (message.ToLower().StartsWith(".hello"))
                        {
                            SendGlobalMessage($"Hello!");
                        }

                        break;
                    }

            }

        }

        public void Register()
        {
            byte[] wearables = { 0, 0, 1, 0, 0, 1, 2, 0 };

            SendToServer(PacketFlags.Reliable, SvPacket.Register, new object[]
            {
                username,
                Animator.StringToHash(password),
                (Int32)skinIndex,
                wearables,
                languageIndex,
                deviceID,
                profileURL
            });
        }

        public void Login()
        {
            SendToServer(PacketFlags.Reliable, SvPacket.Login, new object[]
            {
                    username,
                    Animator.StringToHash(password),
                    languageIndex,
                    deviceID,
                    profileURL
            });
        }


        public void SendToServer(PacketFlags channel, SvPacket packetID, params object[] args)
        {
            // for debugging
            /*
            Log($"{channel} {packetID}:");
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
            Log(arg);
            */


            Buffers.writer.SeekZero();
            Buffers.writer.Write((byte)packetID);
            Buffers.WriteObject(args);

            Packet packet = default(Packet);
            packet.Create(Buffers.writeBuffer, Buffers.writer.Position(), channel);

            connection.CustomSend(ref packet);
        }

        private IEnumerator ConnectionLoop()
        {
            ENet.Event netEvent;

            while (client != null && client.Service(0, out netEvent) >= 0)
            {
                switch (netEvent.Type)
                {

                    case EventType.Connect:
                        Log("Client connected to server");
                        onConnect();
                        break;

                    case EventType.Disconnect:
                        Log("Client disconnected from server");
                        HostCleanup();
                        break;

                    case EventType.Timeout:
                        Log("Client connection timeout");
                        HostCleanup();
                        break;

                    case EventType.Receive:
                        netEvent.Packet.CopyTo(Buffers.readBuffer);
                        Buffers.reader.SeekZero();
                        ClPacket packetID = Buffers.reader.ReadClPacket();

                        if (packetID.ToString() != "UpdateSmooth")
                        Log($"Packet received from server - Type: {packetID}, Channel ID: {netEvent.ChannelID}, Data length: {netEvent.Packet.Length}");
                        handlePacket(packetID);

                        netEvent.Packet.Dispose();
                        break;

                    default:
                        yield return null;
                        break;
                }

            }
            HostCleanup();
        }

        private IEnumerator PlayerLoop()
        {
            while (true)
            {
                SendToServer((PacketFlags)32, SvPacket.UpdatePlayer, new object[]
                {
                    //this.player.GetPosition,
                    //this.player.GetRotation
                });

                yield return new WaitForSeconds(0.125f);
            }
        }

            public void Disconnect()
        {
            connection.DisconnectNow(1);
            if (connectionThread != null)
                Client.instance.StopCoroutine(connectionThread);

            HostCleanup();
        }

        public void HostCleanup()
        {
            client.PreventConnections(true);
            client.Flush();
            client.Dispose();
            client = null;
        }

        public void addOnJoinAction(Action action)
        {
            onJoinActions.Add(action);
        }

        public void Log(string message)
        {
            ConsoleBase.WriteLine($"[BOT] {username}: {message}");
        }

        public void AskForInput()
        {
            Log("[Manual Mode] What should the bot do?: Register / Login / Remove");
        }


    }
}
