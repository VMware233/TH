using System;
using Sirenix.OdinInspector;
using UnityEngine;
using VMFramework.OdinExtensions;
using VMFramework.ResourcesManagement;

namespace TH.Entities
{
    public sealed class GeneralFixedLaserBeamConfig : DamageSourceProjectileConfig, IFixedLaserBeamConfig
    {
        protected override string idSuffix => "laser_beam";

        public override Type gameItemType => typeof(GeneralFixedLaserBeam);

        protected override Type controllerType => typeof(GeneralFixedLaserBeamController);

        [LabelText("射线遮罩"), TabGroup(TAB_GROUP_NAME, PROJECTILE_CATEGORY)]
        public LayerMask impactRaycastMask;

        [LabelText("伤害射线遮罩"), TabGroup(TAB_GROUP_NAME, PROJECTILE_CATEGORY)]
        public LayerMask damageRaycastMask;

        [LabelText("最大射线长度"), Range(0, 100), TabGroup(TAB_GROUP_NAME, PROJECTILE_CATEGORY)]
        public float maxDistance;

        [LabelText("碰撞粒子"), TabGroup(TAB_GROUP_NAME, PROJECTILE_CATEGORY)]
        [GamePrefabID(typeof(ParticlePreset))]
        [IsNotNullOrEmpty]
        public string impactParticleID;
    }
}