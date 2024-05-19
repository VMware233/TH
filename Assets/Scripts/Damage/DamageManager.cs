using Sirenix.OdinInspector;
using TH.Entities;
using TH.GameCore;
using VMFramework.Procedure;

namespace TH.Damage
{
    [ManagerCreationProvider(nameof(GameManagerType.Damage))]
    public class DamageManager : ManagerBehaviour<DamageManager>
    {
        #region Debug

        [Button]
        public static void TakeDamageTo(EntityController source, EntityController target)
        {
            if (source.entity is IDamageSource damageSource)
            {
                damageSource.ForceTakeDamage(target.entity as IDamageable);
            }
        }

        #endregion
    }
}
