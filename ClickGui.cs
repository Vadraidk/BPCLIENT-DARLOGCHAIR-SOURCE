using BrokeProtocolClient.UI;
using BrokeProtocolClient.utils;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using BrokeProtocolClient.settings;

namespace BrokeProtocolClient.modules.render
{
    class ClickGui : Module
    {
        public static ClickGui instance;

        ColorSetting backgroundColor = new ColorSetting("Background", Color.black);
        ColorSetting contentColor = new ColorSetting("Content", Color.cyan);

        List<GuiWindow> windows = new List<GuiWindow>();

        public ClickGui() : base(Categories.Render, "ClickGUI", "Allows to interact with the modules")
        {
            instance = this;

            bind.setBind(Keyboard.current.rightShiftKey.keyCode);
            bind.setDefault(Keyboard.current.rightShiftKey.keyCode);

            addSetting(contentColor);
        }

        public void init()
        {
            foreach (Category category in Modules.instance.getCategories())
            {
                CategoryWindow window = new CategoryWindow(category);
                windows.Add(window);

                foreach (ModuleButton moduleButton in window.moduleButtons)
                {
                    windows.Add(moduleButton.settingWindow);
                }
            }
        }

        public override void onActivate()
        {
            // getClient().ClManager.ToggleCursor();
        }

        public override void onDeactivate()
        {
            //getClient().ClManager.ToggleCursor();
        }

        public override void onRender()
        {
            setSkin();
            foreach (GuiWindow window in windows)
            {
                window.onRender();
            }
        }

        public override void onUpdate()
        {
            //getClient().SceneManager.ShowCursor = true;
            bind.setBind(Keyboard.current.rightShiftKey.keyCode);
            bind.setDefault(Keyboard.current.rightShiftKey.keyCode);
        }

        private void setSkin()
        {
            GUI.backgroundColor = backgroundColor.getColor();
            GUI.contentColor = contentColor.getColor();

            GUI.skin.toggle.onNormal.textColor = Color.green;
            GUI.skin.toggle.onHover.textColor = Color.green;

            Render.StringStyle.wordWrap = true;
        }
    }
}
