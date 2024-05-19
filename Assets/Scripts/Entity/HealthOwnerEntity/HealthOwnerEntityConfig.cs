using System;
using Sirenix.OdinInspector;

namespace TH.Entities
{
    public class HealthOwnerEntityConfig : EntityConfig
    {
        public override Type gameItemType => typeof(HealthOwnerEntity);

        protected override Type controllerType => typeof(HealthOwnerEntityController);

        [LabelText("默认最大生命值"), TabGroup(TAB_GROUP_NAME, BASIC_SETTING_CATEGORY)]
        [MinValue(1)]
        public int defaultMaxHealth = 20;

        [LabelText("自动销毁当生命值为零"), TabGroup(TAB_GROUP_NAME, BASIC_SETTING_CATEGORY)]
        public bool autoDestroyWhenHealthZero = true;

        [LabelText("默认防御力"), TabGroup(TAB_GROUP_NAME, BASIC_SETTING_CATEGORY)]
        [MinValue(0)]
        public int defaultDefense = 0;
        
        [LabelText("默认百分比防御力"), TabGroup(TAB_GROUP_NAME, BASIC_SETTING_CATEGORY)]
        [MinValue(0), MaxValue(1)]
        public float defaultDefensePercent = 0;
    }
}