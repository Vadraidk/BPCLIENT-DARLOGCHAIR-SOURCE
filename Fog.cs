using BrokeProtocolClient.settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BrokeProtocolClient.modules.render
{
    class Fog : Module
    {
        public BooleanSetting removeFog = new BooleanSetting("Remove fog", false);

        public NumberSetting density = new NumberSetting("Fog density", 0, 1, RenderSettings.fogDensity, 0.01);
        public NumberSetting startDistance = new NumberSetting("Fog start distace", 0, 512, RenderSettings.fogStartDistance, 1);
        public NumberSetting endDistance = new NumberSetting("Fog end distace", 0, 512, RenderSettings.fogEndDistance, 1);

        public Fog() : base(Categories.Render, "Fog", "Allows to modify the fog")
        {
            addSetting(removeFog);
            addSetting(density);
            addSetting(startDistance);
            addSetting(endDistance);
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
            RenderSettings.fog = !removeFog.isEnabled();

            RenderSettings.fogDensity = density.getValueFloat();
            RenderSettings.fogStartDistance = startDistance.getValueFloat();
            RenderSettings.fogEndDistance = endDistance.getValueFloat();
        }
    }
}
