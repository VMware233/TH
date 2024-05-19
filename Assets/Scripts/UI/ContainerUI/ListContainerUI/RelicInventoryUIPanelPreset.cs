using System;
using VMFramework.GameLogicArchitecture;

namespace TH.UI
{
    [GamePrefabTypeAutoRegister(ID)]
    public class RelicInventoryUIPanelPreset : ContainerUIPanelPreset
    {
        public const string ID = "relic_inventory_ui";

        public override Type controllerType => typeof(RelicInventoryUIPanelController);
    }
}
