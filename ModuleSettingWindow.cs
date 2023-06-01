using UnityEngine;
using System.Collections.Generic;
using BrokeProtocolClient.modules;
using BrokeProtocolClient.settings;
using BrokeProtocolClient.utils;
using BrokeProtocolClient.UI.setting;

namespace BrokeProtocolClient.UI
{
    class ModuleSettingWindow : GuiWindow
    {
        protected static int index;
        public bool visible = false;

        public Module module;
        public GuiWindow parent;
        protected List<Setting> settings = new List<Setting>();

        private readonly List<setting.Component> components = new List<setting.Component>();

        public ModuleSettingWindow(Module module, GuiWindow parent) : base(module.name)
        {
            this.module = module;
            this.parent = parent;
            this.settings = module.getSettings();
            this.components = new List<setting.Component>();

            int offset = headerPad;

            foreach (Setting setting in settings)
            {
                if (setting is BooleanSetting)
                {
                    components.Add(new CheckBox(setting, this, offset));
                }

                else if(setting is InfoSetting)
                {
                    components.Add(new InfoLabel(setting, this, offset));
                }

                else if (setting is ModeSetting)
                {
                    components.Add(new ModeBox(setting, this, offset));
                }

                else if (setting is NumberSetting)
                {
                    components.Add(new Slider(setting, this, offset));
                    offset += controlHeight + pad;
                    controlIndex++;
                }

                else if (setting is InputSetting)
                {
                    components.Add(new InputBox(setting, this, offset));
                    offset += controlHeight + pad;
                    controlIndex++;
                }

                else if (setting is ColorSetting)
                {
                    components.Add(new ColorPicker(setting, this, offset));
                    offset += controlHeight + pad;
                    controlIndex++;
                }

                else if (setting is BindSetting)
                {
                    components.Add(new Keybinder(setting, this, offset));
                }

                else if (setting is FileSetting)
                {
                    components.Add(new FileExplorer(setting, this, offset));
                }

                else if (setting is ActionSetting)
                {
                    components.Add(new ActionButton(setting, this, offset));
                }

                offset += controlHeight + pad;
                controlIndex++;

            }
            init();
        }

        protected override void init()
        {
            windowSpace = new Rect(parent.windowSpace.x, parent.windowSpace.height, windowSpace.width, windowSpace.height);
            windowIndex = rollingIndex;
            index += 1;
            rollingIndex++;
            ResizeWindowToFitControls();
        }

        protected override void WindowCore()
        {
            foreach (setting.Component component in components)
            {
                component.onRender();
            }
        }

        protected override void WindowWrapper(int id)
        {
            WindowCore();
        }

        public override void onRender()
        {
            if (visible)
            {
                windowSpace = new Rect(parent.windowSpace.x, parent.windowSpace.y + parent.windowSpace.height, windowSpace.width, windowSpace.height);
                windowSpace = GUI.Window(windowIndex, windowSpace, this.WindowWrapper, windowTitle);
            }
        }

        public override void onUpdate()
        {

        }
    }
}
