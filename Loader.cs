using BrokeProtocolClient.modules.render;
using UnityEngine;

namespace BrokeProtocolClient
{
    public class Loader
    {
        public static void Load()
        {
            Loader.Client = new GameObject();
            Loader.Client.AddComponent<Client>();
            UnityEngine.Object.DontDestroyOnLoad(Loader.Client);
        }

        public static void Unload()
        {
            _Unload();
        }

        private static void _Unload()
        {
            GameObject.Destroy(Client);
        }

        private static GameObject Client;
    }
}
