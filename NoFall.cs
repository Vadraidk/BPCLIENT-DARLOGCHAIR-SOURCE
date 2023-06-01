using BrokeProtocol.Entities;
using BrokeProtocolClient.settings;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BrokeProtocolClient.modules.movement
{
    class NoFall : Module
    {
        public NumberSetting range = new NumberSetting("Range", 1, 10, 2, 1);

        public NoFall() : base(Categories.Movement, "No fall damage", "Tries to negate fall damage")
        {
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
            if (getClient().ClManager.myPlayer.ground) return;
            if (getClient().ClManager.myPlayer.isUnderwater) return;

            RaycastHit raycastHit;
            if (Physics.Raycast(getClient().ClManager.myPlayer.GetPosition + Vector3.up * 0.5f, Vector3.down, out raycastHit, range.getValueFloat(), 9217))
            {
                
            }

        }
    }
}
