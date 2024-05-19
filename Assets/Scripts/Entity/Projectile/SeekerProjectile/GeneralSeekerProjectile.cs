using Sirenix.OdinInspector;
using TH.Damage;
using TH.Spells;
using UnityEngine;

namespace TH.Entities
{
    public sealed class GeneralSeekerProjectile : DamageSourceProjectile, ISeekerProjectile
    {
        public struct InitInfo : IDamageSourceProjectileInitInfo, ISeekerProjectileInitInfo
        {
            public Entity sourceEntity;
            public Spell sourceSpell;
            public DamagePacket damagePacket;
            public Transform trackingTarget;
            public Vector2 initialVelocity;

            #region Init Info

            readonly Entity IProjectileInitInfo.sourceEntity => sourceEntity;

            readonly Spell IProjectileInitInfo.sourceSpell => sourceSpell;

            readonly DamagePacket IDamageSourceProjectileInitInfo.damagePacket => damagePacket;

            readonly Transform ISeekerProjectileInitInfo.trackingTarget => trackingTarget;

            readonly Vector2 ISeekerProjectileInitInfo.initialVelocity => initialVelocity;

            #endregion
        }

        private GeneralSeekerProjectileConfig generalSeekerProjectileConfig =>
            gamePrefab as GeneralSeekerProjectileConfig;

        [ShowInInspector]
        public float trackingLerpFactor => generalSeekerProjectileConfig.trackingLerpFactor;

        [ShowInInspector]
        public float maxTrackingVelocity => generalSeekerProjectileConfig.maxTrackingVelocity;

        [ShowInInspector]
        public Transform trackingTarget { get; private set; }

        [ShowInInspector]
        public Vector2 initialVelocity { get; private set; }

        #region Init

        public void Init(InitInfo info)
        {
            InitProjectile(info);
            InitDamageSource(info);

            trackingTarget = info.trackingTarget;
            initialVelocity = info.initialVelocity;
        }

        public void Init(ISeekerProjectileInitInfo initInfo)
        {
            InitProjectile(initInfo);
            
            if (initInfo is IDamageSourceProjectileInitInfo damageSourceInitInfo)
            {
                InitDamageSource(damageSourceInitInfo);
            }

            trackingTarget = initInfo.trackingTarget;
            initialVelocity = initInfo.initialVelocity;
        }

        #endregion
    }
}
