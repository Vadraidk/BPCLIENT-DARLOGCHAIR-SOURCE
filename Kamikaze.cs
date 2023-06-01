using BrokeProtocol.Collections;
using BrokeProtocol.Entities;
using BrokeProtocol.Utility;
using BrokeProtocol.Utility.Networking;
using BrokeProtocolClient.settings;
using ENet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrokeProtocolClient.modules.combat
{
    class Kamikaze : Module
    {
        public ModeSetting mode = new ModeSetting("Mine type", Mode.Any);
        public NumberSetting range = new NumberSetting("Range", 1, 16, 10, 0.5);

        public Kamikaze() : base(Categories.Combat, "Kamikaze", "Drop mine to explode yourself")
        {
            addSetting(mode);
            addSetting(range);
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

            foreach (ShEntity entity in EntityCollections.Entities)
            {
                if (getClient().ClManager.myPlayer.Distance(entity) > range.getValueFloat()) continue;

                ShExplosion explosion = entity as ShExplosion;
                if (!explosion) continue;
                if (!explosion.armed) continue;

                if (shouldPickup(entity)) pickup(entity.ID);
            }
        }

        private bool shouldPickup(ShEntity entity)
        {
            if (mode.isMode((int)Mode.Any) && (entity is ShMineAT || entity is ShMineAP)) return true;
            else if (mode.isMode((int)Mode.AT) && entity is ShMineAT) return true;
            else if (mode.isMode((int)Mode.AP) && entity is ShMineAP) return true;

            return false;
        }

        private void pickup(int id)
        {
            getClient().ClManager.SendToServer((ENet.PacketFlags)1, SvPacket.Collect, new object[]
            {
                id,
                false
            });
        }

        enum Mode
        {
            Any,
            AT,
            AP
        }
    }
}
