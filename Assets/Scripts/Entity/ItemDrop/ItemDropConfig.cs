using System;
using Sirenix.OdinInspector;
using VMFramework.GameLogicArchitecture;

namespace TH.Entities
{
    [GamePrefabTypeAutoRegister(ID)]
    public class ItemDropConfig : EntityConfig
    {
        public const string ID = "item_drop_entity";

        public override Type gameItemType => typeof(ItemDrop);

        [LabelText("最大生命周期"), TabGroup(TAB_GROUP_NAME, BASIC_SETTING_CATEGORY)]
        [MinValue(0.1f), MaxValue(1000f)]
        [Unit(Units.Second)]
        public float maxLifeTime = 60f;
    }
}