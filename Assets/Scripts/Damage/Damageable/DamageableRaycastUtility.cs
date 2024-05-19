using TH.Map;
using UnityEngine;

namespace TH.Damage
{
    public static class DamageableRaycastUtility
    {
        public static bool TryGetDamageable(this RaycastHit2D hit, out IDamageable damageable)
        {
            var damageableController = hit.transform.GetComponent<IDamageableController>();

            if (damageableController != null)
            {
                damageable = damageableController.damageable;
                return true;
            }
        
            if (hit.collider is BoxCollider2D boxCollider2D)
            {
                if (WorldManager.GetCurrentGameMap()
                    .TryGetFloorByGridCollider(boxCollider2D, out var floor))
                {
                    if (floor is IDamageable damageableFloor)
                    {
                        damageable = damageableFloor;
                        return true;
                    }
                }
            }
        
            damageable = null;
            return false;
        }
    }
}