using UnityEngine;
using System.Collections.Generic;
using BrokeProtocolClient.modules;
using BrokeProtocolClient.settings;
using BrokeProtocolClient.utils;

namespace BrokeProtocolClient.UI
{
    class CategoryWindow : GuiWindow
    {
        protected static int index;

        public Category category;

        public readonly List<ModuleButton> moduleButtons = new List<ModuleButton>();
        public CategoryWindow(Category category) : base(category.ToString())
        {
            this.category = category;
            this.windowTitle = category.ToString();

            int offset = headerPad;
            foreach (Module module in Modules.instance.getModulesInCategory(category))
            {
                moduleButtons.Add(new ModuleButton(module, this, offset));
                offset += controlHeight + pad;
                controlIndex++;
            }
            init();
        }

        protected override void init()
        {
            windowSpace = new Rect((controlWidth + pad * 2) * index, 20, 100, 50);
            windowIndex = rollingIndex;
            index += 1;
            rollingIndex++;
            ResizeWindowToFitControls();
        }

        public void hideSettingWindows()
        {
            foreach (ModuleButton moduleButton in moduleButtons)
            {
                moduleButton.settingWindow.visible = false;
            }
        }

        protected override void WindowCore()
        {
            foreach (ModuleButton button in moduleButtons)
            {
                button.onRender();
            }
        }

        protected override void WindowWrapper(int id)
        {
            WindowCore();
            GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));
        }

        public override void onRender()
        {
            windowSpace = GUI.Window(windowIndex, windowSpace, this.WindowWrapper, windowTitle);
        }

        public override void onUpdate()
        {

        }
    }
}
