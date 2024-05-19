using System;
using Sirenix.OdinInspector;

namespace TH.Entities
{
    public abstract class DamageSourceProjectileConfig : ProjectileConfig
    {
        public override Type gameItemType => typeof(DamageSourceProjectile);

        [LabelText("最大伤害次数"), TabGroup(TAB_GROUP_NAME, PROJECTILE_CATEGORY)]
        [MinValue(1)]
        public int maxDamageTimes = 1;
    }
}