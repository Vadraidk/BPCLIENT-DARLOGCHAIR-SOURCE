using BrokeProtocolClient.settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BrokeProtocolClient.modules.misc
{
    class Spoofer : Module
    {
        public BooleanSetting deviceId = new BooleanSetting("Spoof device ID", false);
        public InputSetting deviceIdInput = new InputSetting("Device ID", SystemInfo.deviceUniqueIdentifier.Length, SystemInfo.deviceUniqueIdentifier);

        public BooleanSetting version = new BooleanSetting("Spoof game version", false);
        public InputSetting versionInput = new InputSetting("Version", 10, Client.instance.ShManager.Version);
        ActionSetting resetButton;
        ActionSetting randomButton;

        readonly static System.Random random = new System.Random();
        readonly string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public Spoofer() : base(Categories.Misc, "Spoofer", "Allows to spoof some values")
        {
            addSetting(deviceId);
            addSetting(deviceIdInput);
            resetButton = new ActionSetting("Reset", resetDeviceId);
            addSetting(resetButton);
            randomButton = new ActionSetting("Random", randomDeviceId);
            addSetting(randomButton);

            addSetting(version);
            addSetting(versionInput);
        }

        public override void onActivate()
        {
            
        }

        public override void onDeactivate()
        {
            
        }

        public override void onRender()
        {
            
        }

        public override void onUpdate()
        {
            
        }

        public void resetDeviceId()
        {
            bool state = deviceId.isEnabled();

            deviceId.setEnabled(false);
            deviceIdInput.setValue(SystemInfo.deviceUniqueIdentifier);
            deviceId.setEnabled(state);
        }

        public void randomDeviceId()
        {
            deviceIdInput.setValue(new string(Enumerable.Repeat(chars, SystemInfo.deviceUniqueIdentifier.Length).Select(s => s[random.Next(s.Length)]).ToArray()));
        }

        public string getSpoofedVersion()
        {
            return versionInput.getValue();
        }

        public bool shouldSpoofVersion()
        {
            return version.isEnabled();
        }

        public string getSpoofedDeviceId()
        {
            return deviceIdInput.getValue();
        }

        public bool shouldSpoofDeviceId()
        {
            return deviceId.isEnabled();
        }
    }
}
