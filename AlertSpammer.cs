using BrokeProtocol.Utility.Networking;
using BrokeProtocolClient.settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BrokeProtocolClient.modules.misc
{
    class AlertSpammer : Module
    {
        NumberSetting delay = new NumberSetting("Delay (seconds)", 0, 10, 0.1, 0.1);
        BooleanSetting play = new BooleanSetting("Play alert locally", true);

        IEnumerator spammerThread;

        public AlertSpammer() : base(Categories.Misc, "Alert Spammer", "Spams alerts")
        {
            addSetting(delay);
            addSetting(play);
        }

        public override void onActivate()
        {
            if (spammerThread != null) return;

            spammerThread = SpammerThread();
            getClient().StartCoroutine(spammerThread);
        }

        public override void onDeactivate()
        {
            if (spammerThread == null) return;

            getClient().StopCoroutine(spammerThread);
            spammerThread = null;
        }

        public override void onRender()
        {
            
        }

        public override void onUpdate()
        {
            
        }


        private IEnumerator SpammerThread()
        {
            while (true)
            {
                yield return new WaitForSeconds(delay.getValueFloat());

                getClient().ClManager.SendToServer(ENet.PacketFlags.Reliable, SvPacket.Alert, Array.Empty<object>());

                if (play.isEnabled())
                getClient().ClManager.myPlayer.GetControlled.clMountable.PlayAlert();
            }
        }


    }
}
