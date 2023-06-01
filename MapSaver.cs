using BrokeProtocol.Utility;
using BrokeProtocolClient.settings;
using BrokeProtocolClient.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrokeProtocol.Managers;
using HarmonyLib;

namespace BrokeProtocolClient.modules.misc
{
    class MapSaver : Module
    {
        BooleanSetting useIpName = new BooleanSetting("Name file servers IP", true);
        InputSetting inputName = new InputSetting("Map name", 16, "");

        public MapSaver() : base(Categories.Misc, "Save map", "Saves current map")
        {
            addSetting(useIpName);
            addSetting(inputName);
        }

        public override void onActivate()
        {
            try
            {
                ConsoleBase.WriteLine($"Saving map");
                string filename = useIpName.isEnabled() ? getClient().ClManager.connection.IP : inputName.getValue();
                foreach (char character in System.IO.Path.GetInvalidFileNameChars())
                {
                    filename = filename.Replace(character, '_');
                }

                string filepath = FileManager.MapsPath + filename + "-MapSave.bpm";

                getClient().SceneManager.SaveLevel(filepath, false);
                ConsoleBase.WriteLine($"Map saved in: {filepath}");
            }
            catch (Exception e)
            {
                ConsoleBase.WriteLine($"Cannot download the map! ({e.Message})");
            }
            setEnabled(false);
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

    }
}
