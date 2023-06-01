using BrokeProtocolClient.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BrokeProtocolClient.UI
{
    class DescriptionWindow
    {
        public static string currentTooltip;

        const int padding = 8;

        static Color contentColor = new Color(1, 1, 1, 1);
        static Color backgroundColor = new Color(0, 0, 0, 0.9f);

        public static void render()
        {
            if (currentTooltip.Length <= 0) return;

            //var backgroundPos = Mouse.current.position.ReadValue();
            //backgroundPos = new Vector2(backgroundPos.x, Screen.height - backgroundPos.y);

            var backgroundPos = new Vector2(Screen.width * 0.5f, Screen.height * 0.8f);
            var contentPos = backgroundPos;

            var content = new GUIContent(currentTooltip);
            var contentSize = Render.StringStyle.CalcSize(content);
            var backgroundSize = new Vector2(contentSize.x + padding, contentSize.y + padding);

            Render.DrawBox(backgroundPos, backgroundSize, backgroundColor);
            Render.DrawString(contentPos, currentTooltip, contentColor);
        }

    }
}
