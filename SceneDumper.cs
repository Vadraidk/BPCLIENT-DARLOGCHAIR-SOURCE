using BrokeProtocol.Entities;
using BrokeProtocolClient.settings;

namespace BrokeProtocolClient.modules.exploit
{
    class SceneDumper : Module
    {
        ActionSetting dumpAll;
        ActionSetting dumpWeapons;

        public SceneDumper() : base(Categories.Exploit, "Scene dumper", "")
        {
            dumpAll = new ActionSetting("Dump All", DumpAll);
            addSetting(dumpAll);

            dumpWeapons = new ActionSetting("Dump Weapons", DumpWeapons);
            addSetting(dumpWeapons);
        }

        private void DumpAll()
        {
            foreach (var entity in getClient().SceneManager.entityCollection)
            {
                Log($"{entity.Key}: {entity.Value.name}");
            }
        }

        private void DumpWeapons()
        {
            foreach (var entity in getClient().SceneManager.entityCollection.Values)
            {
                var usable = entity as ShUsable;
                if (!usable) continue;

                if (usable.useDelay == 1f) continue;

                Log($"{usable.itemName} {usable.DamageProperty} {usable.Damage} {usable.useDelay}");
            }
        }

    }
}
