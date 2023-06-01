using UnityEngine;
using BrokeProtocolClient.settings;
using BrokeProtocol.Entities;
using ENet;
using BrokeProtocol.Utility.Networking;

namespace BrokeProtocolClient.modules.combat
{
    class MeleeReach : Module
    {
        NumberSetting maxDistance = new NumberSetting("Max reach", 1, 340, 10, 1);


        public MeleeReach() : base(Categories.Combat, "Melee Reach", "Allows to edit the reach of melee attacks")
        {
            addSetting(maxDistance);
        }


        public void onFire()
        {
            var local = getClient().ClManager.myPlayer;
            if (!local) return;

            var hitscan = local.curEquipable as ShHitscan;
            if (!hitscan) return;

            var hitDir = local.originT.TransformDirection(Quaternion.Euler(hitscan.hitscanOffset.x, hitscan.hitscanOffset.y, 0f) * Vector3.forward);

            RaycastHit hit;
            if (!Physics.SphereCast(local.FutureOrigin, 0.5f, hitDir, out hit, maxDistance.getValueFloat(), hitscan.HitscanMask)) return;

            ShDamageable damageable = hit.collider.GetComponentInParent<ShDamageable>();
            if (!damageable) return;
            if (damageable == local) return;

            getClient().ClManager.SendToServer(PacketFlags.Reliable, SvPacket.CheckHitscan, new object[]
            {
                damageable.ID,
                (byte)0
            });

            Log($"{damageable.name} {damageable.ID} {hitscan.currentBurst}");

        }
    }
}
