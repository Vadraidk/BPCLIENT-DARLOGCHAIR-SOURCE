using BrokeProtocol.Utility.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using BrokeProtocol.Utility;
using Proyecto26;
using BrokeProtocol.Managers;
using BrokeProtocolClient.utils;

namespace BrokeProtocolClient.modules.exploit.botter
{
    class BotManager
    {
        Mode mode = Mode.Auto;

        private readonly HashSet<RequestHelper> requestHelpers = new HashSet<RequestHelper>();

        public List<Bot> Bots = new List<Bot>();


        public BotManager()
        {

        }

        public void AddBot(Bot bot, string hostname, ushort port)
        {
            bot.manager = this;

            Bots.Add(bot);
            bot.Connect(hostname, port);
        }

        public void DisconnectAll()
        {
            Bots.ForEach(bot => { bot.Disconnect(); Bots.Remove(bot); });
        }

        public void DisconnectByName(string username)
        {
            Bots.Where(bot => bot.username == username).ToList().ForEach(bot => { bot.Disconnect(); Bots.Remove(bot); });
        }

        public void SendMessageAll(string message)
        {
            Bots.ForEach(bot => bot.SendGlobalMessage(message));
        }

        public void SendMessageByName(string username, string message)
        {
            Bots.Where(bot => bot.username == username).ToList().ForEach(bot => bot.SendGlobalMessage(message));
        }

        public void Login(string username)
        {
            Bots.Where(bot => bot.username == username).ToList().ForEach(bot => bot.Login());
        }

        public void LoginAll()
        {
            Bots.ForEach(bot => bot.Login());
        }

        public void Register(string username)
        {
            Bots.Where(bot => bot.username == username).ToList().ForEach(bot => bot.Register());
        }

        public void RegisterAll()
        {
            Bots.ForEach(bot => bot.Register());
        }

        public void ConnectRandomServer(Bot bot)
        {
            string url = "https://brokeprotocol.com/servers.json";
            RequestHelper helper = new RequestHelper { Uri = url.RandomURL() };
            requestHelpers.Add(helper);

            RestClient.Get(helper).Then(delegate (ResponseHelper response)
            {
                List<ServerData> sertverList = JsonConvert.DeserializeObject<List<ServerData>>(response.Text.Trim());

                int iteration = 0;
                ServerData server;
                do
                {
                    if (iteration > 100)
                    {
                        ConsoleBase.WriteLine($"[-] Server not found");
                        return;
                    }

                    iteration++;
                    server = Utils.RandomFromList(sertverList);
                } 
                while (server.Whitelist);

                AddBot(bot, server.IP, server.Port);
            })
            .Catch(delegate (Exception e)
            {
                ConsoleBase.WriteLine($"Error getting servers: { (e != null ? e.ToString() : "Unknown") }");
            })
            .Finally(delegate () { requestHelpers.Remove(helper); });

        }

        public void SetMode(Mode mode)
        {
            this.mode = mode;
        }

        public bool IsMode(Mode mode)
        {
            return this.mode == mode;
        }

        public enum Mode
        {
            Auto,
            Manual
        }

    }
}
