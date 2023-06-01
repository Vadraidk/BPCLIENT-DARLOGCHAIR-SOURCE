using BrokeProtocol.Client.UI;
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

namespace BrokeProtocolClient.modules.exploit
{
    class AutoHackAppartment : Module
    {

        public IEnumerator cracker;

        public AutoHackAppartment() : base(Categories.Exploit, "Auto Hack Appart", "Automatically cracks appart")
        {

        }

        public override void onActivate()
        {

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
            while (true)
            {
                
            }

            yield return new WaitForSeconds(delay);
        }
    }
}
