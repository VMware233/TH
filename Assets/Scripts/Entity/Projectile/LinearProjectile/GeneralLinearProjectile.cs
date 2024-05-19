using Sirenix.OdinInspector;
using TH.Damage;
using TH.Spells;
using UnityEngine;

namespace TH.Entities
{
    public sealed class GeneralLinearProjectile : DamageSourceProjectile, ILinearProjectile
    {
        public struct InitInfo : ILinearProjectileInitInfo, IDamageSourceProjectileInitInfo
        {
            public Entity sourceEntity;
            public Spell sourceSpell;
            public DamagePacket damagePacket;
            public Vector2 initialVelocity;

            #region Init Info

            readonly Entity IProjectileInitInfo.sourceEntity => sourceEntity;

            readonly Spell IProjectileInitInfo.sourceSpell => sourceSpell;

            readonly DamagePacket IDamageSourceProjectileInitInfo.damagePacket => damagePacket;

            readonly Vector2 ILinearProjectileInitInfo.initialVelocity => initialVelocity;

            #endregion
        }

        [ShowInInspector]
        public Vector2 initialVelocity { get; private set; }

        #region Init

        public void Init(InitInfo info)
        {
            InitProjectile(info);
            InitDamageSource(info);

            initialVelocity = info.initialVelocity;
        }
        
        public void Init(ILinearProjectileInitInfo info) 
        {
            InitProjectile(info);

            if (info is IDamageSourceProjectileInitInfo damageSourceInfo)
            {
                InitDamageSource(damageSourceInfo);
            }

            initialVelocity = info.initialVelocity;
        }

        #endregion
    }
}
