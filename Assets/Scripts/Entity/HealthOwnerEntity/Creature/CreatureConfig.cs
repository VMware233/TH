using System;
using Sirenix.OdinInspector;

namespace TH.Entities
{
    public class CreatureConfig : HealthOwnerEntityConfig
    {
        #region Categories

        protected const string CREATURE_CATEGORY = "生物设置";

        #endregion

        public override Type gameItemType => typeof(Creature);

        protected override Type controllerType => typeof(CreatureController);

        [LabelText("默认移速"), TabGroup(TAB_GROUP_NAME, CREATURE_CATEGORY)]
        [MinValue(0)]
        public float defaultMovementSpeed = 10;

        [LabelText("默认攻击"), LabelWidth(100), TabGroup(TAB_GROUP_NAME, CREATURE_CATEGORY)]
        [MinValue(0)]
        public int defaultAttack = 1;
        
        [LabelText("默认总攻击倍率"), LabelWidth(100), TabGroup(TAB_GROUP_NAME, CREATURE_CATEGORY)]
        [MinValue(0)]
        public int defaultAttackMultiplier = 0;
        
        [LabelText("默认暴击率"), LabelWidth(100), TabGroup(TAB_GROUP_NAME, CREATURE_CATEGORY)]
        [PropertyRange(0, 1)]
        public float defaultCriticalRate = 0.05f;
        
        [LabelText("默认暴击倍率"), LabelWidth(100), TabGroup(TAB_GROUP_NAME, CREATURE_CATEGORY)]
        [MinValue(1)]
        public float defaultCriticalDamageMultiplier = 1.5f;
    }
}