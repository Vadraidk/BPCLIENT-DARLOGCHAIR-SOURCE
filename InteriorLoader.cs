using BrokeProtocolClient.settings;
using BrokeProtocolClient.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace BrokeProtocolClient.modules.exploit
{
    class InteriorLoader : Module
    {
        InputSetting interiorId = new InputSetting("Interior ID", 4, "0");
        ActionSetting setButton;
        ActionSetting currentButton;
        ActionSetting allButton;

        public InteriorLoader() : base(Categories.Exploit, "Interior Loader", "Loads specified interior")
        {
            addSetting(interiorId);

            setButton = new ActionSetting("Set interior ID", SetInterior);
            addSetting(setButton);

            currentButton = new ActionSetting("Curren interior", CurrentInterior);
            addSetting(currentButton);

            allButton = new ActionSetting("All interiors", AllInteriors);
            addSetting(allButton);
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

        private void SetInterior()
        {
            getClient().SceneManager.SetInterior(Int32.Parse(interiorId.getValue()));
        }

        private void CurrentInterior()
        {
            ConsoleBase.WriteLine($"Current interior:\nName: {getClient().SceneManager.mTransform.GetChild(getClient().SceneManager.currentInterior).name} ID: {getClient().SceneManager.currentInterior}");
        }

        private void AllInteriors()
        {
            ConsoleBase.WriteLine($"All interiors:");

            for (int i = 0; i < getClient().SceneManager.mTransform.childCount; i++)
            {
                ConsoleBase.WriteLine($"Name: {getClient().SceneManager.mTransform.GetChild(i).name} ID: {getClient().SceneManager.mTransform.GetChild(i).GetSiblingIndex()}");
            }

        }
    }
}
