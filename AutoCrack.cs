using BrokeProtocol.Entities;
using BrokeProtocol.Utility.Networking;
using BrokeProtocolClient.settings;
using BrokeProtocolClient.utils;
using ENet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BrokeProtocolClient.modules.player
{
    class AutoCrack : Module
    {

        public IEnumerator cracker;

        public AutoCrack() : base(Categories.Player, "Auto Crack", "Automatically cracks containers")
        {

        }

        public override void onActivate()
        {
            /*
            if (!getClient().ClManager.highlightEntity) return;

            getClient().ClManager.SendToServer((PacketFlags)1, SvPacket.View, new object[]
            {
                getClient().ClManager.highlightEntity.ID,
                true
            });

            return;
            */

            if (cracker != null) return;

            cracker = Cracker(0.1f);
            getClient().StartCoroutine(cracker);
        }

        public override void onDeactivate()
        {
            if (cracker == null) return;

            getClient().StopCoroutine(cracker);
            cracker = null;
        }

        public override void onRender()
        {

        }

        public override void onUpdate()
        {

        }

        private IEnumerator Cracker(float delay)
        {
            ShEntity target = getClient().ClManager.highlightEntity;

            while (canCrack(target))
            {
                getClient().ClManager.SendToServer(PacketFlags.Reliable, SvPacket.CrackStart, new object[]
                {
                    target.ID
                });

                for (int i = 0; i < 4; i++)
                {
                    float solution = (float)((Math.PI / 2 * i) - Math.PI / 2);

                    getClient().ClManager.SendToServer(PacketFlags.Reliable, SvPacket.CrackAttempt, new object[]
                    {
                        solution
                    });
                }

                yield return new WaitForSeconds(delay);
            }
        }

        private bool canCrack(ShEntity target)
        {
            if (!getClient().ClManager.myPlayer) return false;
            if (!target) return false;
            //if (!getClient().ClManager.myPlayer.HasItem(getClient().ShManager.lockpick)) return false;

            return true;
        }
    }
}
