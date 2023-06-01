

using BrokeProtocol.Entities;
using BrokeProtocolClient.settings;
using BrokeProtocolClient.utils;
using UnityEngine;

namespace BrokeProtocolClient.modules.combat
{
    class NoSpread : Module
    {
        ActionSetting dumpVectors;

        public Quaternion originalRotation;

        public NoSpread() : base(Categories.Combat, "No Spread", "Makes weapons have no bullet spread")
        {
            dumpVectors = new ActionSetting("Dump vectors", DumpVectors);
            addSetting(dumpVectors);
        }

        public void PreCancelSpread()
        {
            var local = getClient().ClManager.myPlayer;
            if (!local) return;

            ShGun gun = local.curEquipable as ShGun;
            if (!gun) return;

            var negatedSpreadVector = local.originT.forward + (local.originT.forward - gun.NextFireVector().normalized);

            Quaternion bestRotation = Quaternion.LookRotation(negatedSpreadVector);

            originalRotation = local.GetRotation;
            local.SetRotation(bestRotation);
        }

        public void PostCancelSpread()
        {
            var local = getClient().ClManager.myPlayer;
            if (!local) return;

            ShGun gun = local.curEquipable as ShGun;
            if (!gun) return;

            local.SetRotation(originalRotation);
        }

        void DumpVectors()
        {
            int index = 0;
            foreach (var vec in getClient().ShManager.randomVector)
            {
                Log($"{index} {vec}");
                index++;
            }
        }

    }
}
