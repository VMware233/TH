using VMFramework.Core;
using Sirenix.OdinInspector;
using TH.Damage;
using UnityEngine;
using VMFramework.Procedure;

namespace TH.Entities
{
    public abstract class Projectile : Entity, IDamageSource, IProjectile
    {
        protected ProjectileConfig projectileConfig => gamePrefab as ProjectileConfig;

        [ShowInInspector]
        public Entity sourceEntity { get; private set; }

        [ShowInInspector]
        public float maxLifeTime => projectileConfig.maxLifeTime;

        [ShowInInspector]
        private float timer = 0;

        #region Init

        protected override void OnInit()
        {
            base.OnInit();

            if (isServer)
            {
                UpdateDelegateManager.AddUpdateDelegate(UpdateType.Update, UpdateOnServer);
            }
        }

        public void InitProjectile<TInitInfo>(TInitInfo initInfo) where TInitInfo : IProjectileInitInfo
        {
            sourceEntity = initInfo.sourceEntity;
        }

        #endregion

        #region Update

        private void UpdateOnServer()
        {
            timer += Time.deltaTime;

            if (timer >= maxLifeTime)
            {
                EntityManager.DestroyEntity(this);
            }
        }

        #endregion

        #region Destroy

        protected override void OnDestroy()
        {
            base.OnDestroy();

            UpdateDelegateManager.RemoveUpdateDelegate(UpdateType.Update, UpdateOnServer);
        }

        #endregion
    }
}
