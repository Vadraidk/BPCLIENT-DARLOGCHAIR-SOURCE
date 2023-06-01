using BrokeProtocol.Utility.Networking;
using BrokeProtocolClient.settings;
using BrokeProtocolClient.utils;
using ENet;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using BrokeProtocolClient.modules.render;

namespace BrokeProtocolClient.modules
{
    abstract class Module
    {
        public readonly Category category;
        public readonly string name;
        public readonly string description;

        public readonly List<Setting> settings = new List<Setting>();

        public bool enabled;

        public BindSetting bind = new BindSetting(0, "Bind");

        public Module(Category category, string name, string description)
        {
            this.category = category;
            this.name = name;
            this.description = description;

            addSetting(bind);

            if (PlayerPrefs.HasKey(name))
            {
                setEnabled(PlayerPrefs.GetInt(name) != 0);
            }
        }

        public virtual void onActivate() { }
        public virtual void onUpdate() { }
        public virtual void onRender() { }
        public virtual void onDeactivate() { }

        public virtual bool onSendToServer(PacketFlags channel, SvPacket packet, params object[] args) { return true; }


        public void addSetting(Setting setting)
        {
            settings.Add(setting);
        }

        public List<Setting> getSettings()
        {
            return settings;
        }

        public void toggle()
        {
            if (enabled)
            {
                enabled = false;
                Modules.instance.removeActive(this);
                onDeactivate();
            }
            else
            {
                enabled = true;
                Modules.instance.addActive(this);
                onActivate();
            }



                PlayerPrefs.SetInt(name, this.enabled ? 1 : 0);
                PlayerPrefs.Save();
            
        }
        public void setEnabled(bool enabled)
        {
            this.enabled = enabled;
            if (enabled)
            {
                Modules.instance.addActive(this);
                onActivate();
            }
            else
            {
                Modules.instance.removeActive(this);
                onDeactivate();
            }

            PlayerPrefs.SetInt(name, this.enabled ? 1 : 0);
            PlayerPrefs.Save();
        }

        protected Client getClient()
        {
            return Client.instance;
        }

        public Key getKeyBind()
        {
            return bind.getKey();
        }

        public string getName()
        {
            return name;
        }

        public bool isEnabled()
        {
            return enabled;
        }

        protected void Log(string message)
        {
            ConsoleBase.WriteLine($"[{DateTime.Now.ToString("h:mm:ss tt")}] [Module {getName()}]: {message}");
        }


    }
}
