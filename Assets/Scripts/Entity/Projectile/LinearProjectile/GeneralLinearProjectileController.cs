using UnityEngine;

namespace TH.Entities
{
    public sealed class GeneralLinearProjectileController : GeneralDamageSourceProjectileController
    {
        public GeneralLinearProjectile linearProjectile => (GeneralLinearProjectile)entity;

        private new Rigidbody2D rigidbody2D;

        private void Awake()
        {
            rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            if (IsServerStarted == false || linearProjectile == null)
            {
                return;
            }

            rigidbody2D.velocity = linearProjectile.initialVelocity;
        }
    }
}
