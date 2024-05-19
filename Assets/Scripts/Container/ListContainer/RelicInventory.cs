using System;
using TH.Items;
using VMFramework.Containers;
using VMFramework.GameLogicArchitecture;

namespace TH.Containers
{
    public class RelicInventory : ListContainer
    {
        public override bool TryAddItem(IContainerItem item)
        {
            if (item is not Relic)
            {
                return false;
            }

            return base.TryAddItem(item);
        }
    }

    [GamePrefabTypeAutoRegister(ID)]
    public class RelicInventoryPreset : ListContainerPreset
    {
        public const string ID = "relic_inventory_container";

        public override Type gameItemType => typeof(RelicInventory);
    }
}
