using BrokeProtocol.Client.UI;
using BrokeProtocolClient.settings;
using BrokeProtocolClient.utils;
using HarmonyLib;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace BrokeProtocolClient.modules.misc
{
    class BetterChat : Module
    {
        BooleanSetting localChat = new BooleanSetting("Show local chat", true);
        BooleanSetting infiniteChatHistory = new BooleanSetting("Infinite chat history", true);

        List<string> chatHistory = new List<string>();
        List<string> sentHistory = new List<string>();
        int index = 0;

        static AccessTools.FieldRef<ChatMenu, InputField> inputFieldRef = AccessTools.FieldRefAccess<ChatMenu, InputField>("submitInput");

        public BetterChat() : base(Categories.Misc, "Better Chat", "Improves chat")
        {
            addSetting(localChat);
            addSetting(infiniteChatHistory);
        }

        public override void onUpdate()
        {
            ChatMenu chatMenu = getClient().ClManager.CurrentMenu as ChatMenu;
            if (!chatMenu)
            {
                index = 0;
                return;
            }

            if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                index++;
                index = Mathf.Clamp(index, 0, sentHistory.Count - 1);
                inputFieldRef(chatMenu).text = sentHistory[index];
            }

            if (Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                index--;
                index = Mathf.Clamp(index, 0, sentHistory.Count - 1);
                inputFieldRef(chatMenu).text = sentHistory[index];
            }
        }

        public bool shouldLogLocalChat()
        {
            return localChat.isEnabled();
        }

        public bool isInfinite()
        {
            return infiniteChatHistory.isEnabled();
        }

        public void addToHistory(string message)
        {
            sentHistory.Insert(0, message);
        }

        public void AppendChatHistory(string message)
        {
            chatHistory.Add(message);
        }

        public string GetChatHistory()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string message in chatHistory)
            {
                stringBuilder.AppendLine(message);
            }
            return stringBuilder.ToString();
        }

    }
}
