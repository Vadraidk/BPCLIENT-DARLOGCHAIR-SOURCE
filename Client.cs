using UnityEngine;
using BrokeProtocol.Managers;
using BrokeProtocol.Utility;
using BrokeProtocolClient.modules;
using BrokeProtocolClient.modules.render;
using BrokeProtocolClient.UI;
using BrokeProtocolClient.utils;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using System.Collections;
using HarmonyLib;
using BrokeProtocol.Client.UI;
using HUD = BrokeProtocolClient.modules.render.HUD;

namespace BrokeProtocolClient 
{
    class Client : MonoBehaviour
    {
        public static Client instance;
        public FileManager FileManager;
        public ClManager ClManager = MonoBehaviourSingleton<ClManager>.Instance;
        public ShManager ShManager = MonoBehaviourSingleton<ShManager>.Instance;
        public SceneManager SceneManager = MonoBehaviourSingleton<SceneManager>.Instance;
        public MainCamera MainCamera = MonoBehaviourSingleton<MainCamera>.Instance;
        public MinigamePanel miniGamePanel = FindObjectOfType<MinigamePanel>();

        private Harmony harmony = new Harmony("BrokeProtocolClient");

        public Color Rainbow = Color.white;

        public void Start()
        {
            try
            {
                ConsoleBase.WriteLine("[*] Injecting...");
                instance = this;
                FileManager = new FileManager();

                ConsoleBase.WriteLine("[*] Patching...");
                harmony.PatchAll();

                ConsoleBase.WriteLine("[*] Initializing modules...");
                new Modules();
                Categories.init();

                ConsoleBase.WriteLine("[*] Initializing UI...");
                Modules.instance.add(new ClickGui());
                ClickGui.instance.init();
                ClickGui.instance.setEnabled(true);

                //StartCoroutine(OnKey());

                ConsoleBase.WriteLine("[+] Successfully injected!");
                ClManager.ShowMessage($"<color=#00FF00>[+]</color> <color=#00ccff>{HUD.ClientLogo}</color> <color=#cc00ff>Successfully injected!</color>");

                //gameObject.AddComponent<ClickImGui>();
            }
            catch (System.Exception e)
            {
                ConsoleBase.WriteLine($"[-] Failed to inject, error:\n{e.ToString()}");
            }


        }

        public void Update()
        {
            if (!ClManager) ClManager = MonoBehaviourSingleton<ClManager>.Instance;
            if (!ShManager) ShManager = MonoBehaviourSingleton<ShManager>.Instance;
            if (!SceneManager) SceneManager = MonoBehaviourSingleton<SceneManager>.Instance;
            if (!MainCamera) MainCamera = MonoBehaviourSingleton<MainCamera>.Instance;
            //if (!miniGamePanel) miniGamePanel = FindObjectOfType<MinigamePanel>();

            Rainbow = Color.HSVToRGB(Mathf.PingPong(Time.time * 0.1f, 1), 1, 1);

            foreach (Module module in Modules.instance.getActive())
            {
                module.onUpdate();
            }

            const string _ = "why u looking my code fuck u";
            bool __ = true;
            __ = false;
            if (__) ConsoleBase.WriteLine(_);
            onKey();



        }

        public void OnGUI()
        {
            foreach (Module module in Modules.instance.getActive())
            {
                module.onRender();
            }

            InfoUtils.onRender();

        }

        private void onKey()
        {
            if (ClManager.IsTyping()) return;

            foreach (KeyControl key in Keyboard.current.allKeys)
            {
                if (!key.wasPressedThisFrame) continue;
                if (ClickGui.instance.isEnabled() && key.keyCode != ClickGui.instance.getKeyBind()) continue;

                foreach (Module module in Modules.instance.getModules())
                {
                    if (module.getKeyBind() == key.keyCode)
                    {
                        module.toggle();
                    }
                }
            }
        }

    }
}
