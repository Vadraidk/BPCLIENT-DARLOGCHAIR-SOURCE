using BrokeProtocol.Collections;
using BrokeProtocol.Entities;
using BrokeProtocolClient.settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BrokeProtocolClient.modules.exploit
{
    class HitboxExpander : Module
    {
        NumberSetting size = new NumberSetting("Hitbox size", 0.1, 5, 1.5, 0.1);
        public HitboxExpander() : base(Categories.Exploit, "Hitbox expander", "Expands player hitboxes")
        {
            addSetting(size);
        }

        public override void onUpdate()
        {
            foreach (ShPlayer player in EntityCollections.Players)
            {
                player.gameObject.transform.localScale = Vector3.one * size.getValueFloat();
            }
        }
    }
}
