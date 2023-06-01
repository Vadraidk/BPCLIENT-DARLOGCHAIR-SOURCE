using UnityEngine;
using System.Collections.Generic;
using BrokeProtocolClient.modules;
using BrokeProtocolClient.settings;
using BrokeProtocolClient.utils;

namespace BrokeProtocolClient.UI
{
    class ModuleButton
    {
        public Module module;
        public GuiWindow parent;
        public int offset;
        private bool value;
        private bool lastValue;
        public bool settingValue;

        public ModuleSettingWindow settingWindow;

        public ModuleButton(Module module, GuiWindow parent, int offset)
        {
            this.module = module;
            this.parent = parent;
            this.offset = offset;
            this.value = module.isEnabled();
            this.lastValue = module.isEnabled();
            this.settingValue = false;

            settingWindow = new ModuleSettingWindow(module, parent);
        }

        public void onRender()
        {
            value = GUI.Toggle(new Rect(parent.pad, offset, parent.controlWidth * 0.75f, parent.controlHeight), module.isEnabled(), new GUIContent(module.name, module.description));
            if (value != lastValue)
            {
                module.setEnabled(value);
            }
            lastValue = value;


            if (GUI.Button(new Rect(parent.pad + parent.controlWidth - parent.controlWidth * 0.25f, offset, parent.controlWidth * 0.25f, parent.controlHeight), $"config"))
            {
                var hidden = settingWindow.visible;
                (parent as CategoryWindow).hideSettingWindows();
                settingWindow.visible = !hidden;
            }

            if (GUI.tooltip.Length > 0)
                DescriptionWindow.currentTooltip = GUI.tooltip;
        }
    }
}
