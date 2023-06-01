using BrokeProtocolClient.settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrokeProtocolClient.modules.player
{
    class FovChanger : Module
    {
        public NumberSetting fov = new NumberSetting("Fov", 50, 180, 90, 5);

        private float defaultFov = 75f;

        public FovChanger() : base(Categories.Player, "Fov Changer", "Changes your field of view")
        {
            addSetting(fov);
        }

        public override void onActivate()
        {
            getClient().MainCamera.defaultFOV = fov.getValueFloat();
        }

        public override void onDeactivate()
        {
            getClient().MainCamera.defaultFOV = defaultFov;
        }

        public override void onRender()
        {
            
        }

        public override void onUpdate()
        {
            
        }
    }
}
