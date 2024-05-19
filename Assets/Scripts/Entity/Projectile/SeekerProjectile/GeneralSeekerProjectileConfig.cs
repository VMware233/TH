using System;
using Sirenix.OdinInspector;

namespace TH.Entities
{
    public sealed class GeneralSeekerProjectileConfig : DamageSourceProjectileConfig, ISeekerProjectileConfig
    {
        public override Type gameItemType => typeof(GeneralSeekerProjectile);

        protected override Type controllerType => typeof(GeneralSeekerProjectileController);

        [LabelText("追踪插值因子"), TabGroup(TAB_GROUP_NAME, PROJECTILE_CATEGORY)]
        [PropertyRange(0, 1)]
        public float trackingLerpFactor = 0.1f;

        [LabelText("最大跟踪速度"), TabGroup(TAB_GROUP_NAME, PROJECTILE_CATEGORY)]
        public float maxTrackingVelocity = 25f;
    }
}