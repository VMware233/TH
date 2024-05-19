using System;
using Sirenix.OdinInspector;

namespace TH.Map
{
    public class DamageableFloorPreset : FloorPreset
    {
        public override Type gameItemType => typeof(DamageableFloor);

        protected override Type floorControllerType => typeof(DamageableFloorController);

        [LabelText("默认生命值"), TabGroup(TAB_GROUP_NAME, BASIC_SETTING_CATEGORY)]
        [MinValue(0)]
        public int defaultHealth;

        [LabelText("忽略投射物"), TabGroup(TAB_GROUP_NAME, BASIC_SETTING_CATEGORY)]
        public bool ignoreProjectile = false;
    }
}