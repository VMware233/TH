using System;
using Sirenix.OdinInspector;
using TH.Damage;
using TH.Entities;
using VMFramework.Property;

namespace TH.Map
{
    public class DamageableFloor : Floor, IDamageable
    {
        protected DamageableFloorPreset damageableFloorPreset => (DamageableFloorPreset)gamePrefab;

        [ShowInInspector]
        public BaseIntProperty health;

        #region Init

        protected override void OnCreate()
        {
            base.OnCreate();

            health = new BaseIntProperty(damageableFloorPreset.defaultHealth);

            health.OnValueChanged += OnHealthChanged;
        }

        #endregion

        #region Health

        private void OnHealthChanged(int previous, int current)
        {
            if (isDestroyed)
            {
                return;
            }

            if (isClient)
            {
                DamageUIUtility.PopupHealthChange(current - previous, tilePivotRealPos);
            }

            if (current <= 0)
            {
                gameMapNetwork.DestroyBlockOnServer(tile.xy, new FloorDestructionInfo()
                {
                    enableDroppings = true
                });
            }
        }

        #endregion

        #region Damageable

        public event Action<DamageResult> OnDamageTaken;

        void IDamageable.ProcessDamageResult(DamageResult result)
        {
            ProcessDamageResult(result);
            
            OnDamageTaken?.Invoke(result);
        }
        
        public virtual void ProcessDamageResult(DamageResult result)
        {
            health.value += result.healthChange;
        }

        bool IDamageable.CanBeDamaged(IDamageSource source)
        {
            if (damageableFloorPreset.ignoreProjectile)
            {
                if (source is Projectile)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion
    }
}
