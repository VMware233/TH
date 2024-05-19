using Sirenix.OdinInspector;
using TH.Damage;

namespace TH.Entities
{
    public abstract class DamageSourceProjectile : Projectile, IDamageSource, IDamageSourceProjectile
    {
        protected DamageSourceProjectileConfig damageSourceProjectileConfig =>
            (DamageSourceProjectileConfig)gamePrefab;

        [ShowInInspector]
        public int maxDamageTimes => damageSourceProjectileConfig.maxDamageTimes;

        [ShowInInspector]
        public DamagePacket damagePacket { get; private set; }

        [ShowInInspector]
        private int currentDamageTimes = 0;

        #region Init

        protected override void OnCreate()
        {
            base.OnCreate();

            currentDamageTimes = 0;
        }

        protected void InitDamageSource<TInitInfo>(TInitInfo initInfo)
            where TInitInfo : IDamageSourceProjectileInitInfo
        {
            var damagePacket = initInfo.damagePacket;
            damagePacket.directSource = this;
            this.damagePacket = damagePacket;
        }

        #endregion

        #region Damage

        void IDamageSource.ProduceDamagePacket(IDamageable target, out DamagePacket packet)
        {
            packet = damagePacket;

            currentDamageTimes++;

            if (currentDamageTimes >= maxDamageTimes)
            {
                EntityManager.DestroyEntity(this);
            }
        }

        public bool CanDamage(IDamageable target)
        {
            return currentDamageTimes < maxDamageTimes;
        }

        #endregion
    }
}
