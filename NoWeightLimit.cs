using BrokeProtocolClient.settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrokeProtocolClient.modules.player
{
    class NoWeightLimit : Module
    {
        public BooleanSetting infinite = new BooleanSetting("Infinite weight limit", true);
        public NumberSetting limit = new NumberSetting("Weight limit", 0, 1000, 200, 1);

        public NoWeightLimit() : base(Categories.Player, "No weight limit", "Allows to change your players weight limit")
        {
            addSetting(infinite);
            addSetting(limit);
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
            if (!getClient().ClManager.myPlayer) return;

            getClient().ClManager.myPlayer.weightLimit = infinite.isEnabled() ? float.PositiveInfinity : limit.getValueFloat();
        }
    }
}
