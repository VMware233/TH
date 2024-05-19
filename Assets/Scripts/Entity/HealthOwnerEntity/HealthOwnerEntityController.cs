using FishNet.Object.Synchronizing;
using Sirenix.OdinInspector;
using TH.Damage;

namespace TH.Entities
{
    public class HealthOwnerEntityController : EntityController, IDamageableController
    {
        public HealthOwnerEntity healthOwnerEntity => entity as HealthOwnerEntity;

        protected override void OnPreInit()
        {
            base.OnPreInit();
            
            healthOnServer.OnChange += OnHealthOnServerChanged;
        }

        protected override void OnPostInit()
        {
            base.OnPostInit();
            
            if (IsServerStarted)
            {
                healthOnServer.Value = healthOwnerEntity.health.value;
                
                healthOwnerEntity.health.OnValueChanged += OnHealthChangedOnServer;
            }
        }

        public override void OnStopServer()
        {
            base.OnStopServer();

            healthOwnerEntity.health.OnValueChanged -= OnHealthChangedOnServer;
        }

        #region Health

        [ShowInInspector]
        protected readonly SyncVar<int> healthOnServer =
            new(new SyncTypeSettings(WritePermission.ServerOnly, ReadPermission.Observers));

        private void OnHealthOnServerChanged(int previous, int current, bool asServer)
        {
            if (asServer || healthOwnerEntity == null || IsServerStarted)
            {
                return;
            }

            healthOwnerEntity.health.value = current;
        }

        private void OnHealthChangedOnServer(int previous, int current)
        {
            healthOnServer.Value = current;
        }

        #endregion

        #region Damageable Controller

        IDamageable IDamageableController.damageable => healthOwnerEntity;

        #endregion
    }
}
