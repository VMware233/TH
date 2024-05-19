using TH.Damage;
using TH.Map;
using UnityEngine;

namespace TH.Entities
{
    public class GeneralDamageSourceProjectileController : ProjectileController
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (IsServerStarted == false)
            {
                return;
            }

            var damageableController = other.GetComponent<IDamageableController>();

            if (damageableController == null)
            {
                if (other is not BoxCollider2D boxCollider2D)
                {
                    return;
                }

                if (WorldManager.GetCurrentGameMap()
                        .TryGetFloorByGridCollider(boxCollider2D, out var floor) == false)
                {
                    return;
                }

                if (floor is not IDamageable damageable)
                {
                    return;
                }

                projectile.RequestTakeDamage(damageable);
                return;
            }

            if (damageableController.damageable == projectile.sourceEntity)
            {
                return;
            }

            projectile.RequestTakeDamage(damageableController.damageable);
        }
    }
}
