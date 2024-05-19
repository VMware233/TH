using FishNet.Serializing;
using Sirenix.OdinInspector;
using TH.Damage;
using TH.Spells;
using UnityEngine;

namespace TH.Entities
{
    public sealed class GeneralFixedLaserBeam : DamageSourceProjectile, IFixedLaserBeam
    {
        public struct InitInfo : IFixedLaserBeamInitInfo, IDamageSourceProjectileInitInfo
        {
            public Entity sourceEntity;
            public Spell sourceSpell;
            public DamagePacket damagePacket;
            public Vector2 direction;

            #region Init Info

            readonly Entity IProjectileInitInfo.sourceEntity => sourceEntity;

            readonly Spell IProjectileInitInfo.sourceSpell => sourceSpell;

            readonly DamagePacket IDamageSourceProjectileInitInfo.damagePacket => damagePacket;

            Vector2 IFixedLaserBeamInitInfo.direction => direction;

            #endregion
        }

        private GeneralFixedLaserBeamConfig generalFixedLaserBeamConfig => 
            (GeneralFixedLaserBeamConfig)gamePrefab;

        public LayerMask impactRaycastMask => generalFixedLaserBeamConfig.impactRaycastMask;

        public LayerMask damageRaycastMask => generalFixedLaserBeamConfig.damageRaycastMask;

        public float maxDistance => generalFixedLaserBeamConfig.maxDistance;

        public string impactParticleID => generalFixedLaserBeamConfig.impactParticleID;

        [ShowInInspector]
        public Vector2 direction { get; private set; }

        #region Init

        public void Init(InitInfo info)
        {
            InitProjectile(info);
            InitDamageSource(info);
            
            direction = info.direction;
        }

        public void Init(IFixedLaserBeamInitInfo initInfo)
        {
            InitProjectile(initInfo);
            
            if (initInfo is IDamageSourceProjectileInitInfo damageSourceInitInfo)
            {
                InitDamageSource(damageSourceInitInfo);
            }

            direction = initInfo.direction;
        }

        #endregion

        #region Net Serialization

        protected override void OnWrite(Writer writer)
        {
            base.OnWrite(writer);

            writer.WriteVector2(direction);
        }

        protected override void OnRead(Reader reader)
        {
            base.OnRead(reader);

            direction = reader.ReadVector2();
        }

        #endregion
    }
}
