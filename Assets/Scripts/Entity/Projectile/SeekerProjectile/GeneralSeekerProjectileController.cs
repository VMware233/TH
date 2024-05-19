using Sirenix.OdinInspector;
using TH.Utilities;
using UnityEngine;
using VMFramework.Core;

namespace TH.Entities
{
    public sealed class GeneralSeekerProjectileController : GeneralDamageSourceProjectileController
    {
        #region Config

        [SerializeField]
        private Vector2 trackingSize = new Vector2(8, 8);

        #endregion

        public GeneralSeekerProjectile seekerProjectile => (GeneralSeekerProjectile)entity;

        [ShowInInspector]
        private Transform target;

        [ShowInInspector]
        private Vector2 currentVelocity;

        private new Rigidbody2D rigidbody2D;

        private void Awake()
        {
            rigidbody2D = GetComponent<Rigidbody2D>();
        }

        protected override void OnInit()
        {
            base.OnInit();

            target = seekerProjectile.trackingTarget;

            currentVelocity = seekerProjectile.initialVelocity;
        }

        private void FixedUpdate()
        {
            if (IsServerStarted == false || seekerProjectile == null)
            {
                return;
            }

            if (target == null)
            {
                rigidbody2D.velocity = currentVelocity;

                // 临时代码，用于测试
                if (seekerProjectile.sourceEntity is Player player)
                {
                    var colliders = Physics2D.OverlapBoxAll(transform.position, trackingSize, 0,
                        LayerManager.entityLayerMask);

                    if (colliders.Length <= 0)
                    {
                        return;
                    }

                    target = colliders.Choose().transform;
                }

                return;
            }

            var targetDirection = (target.position - transform.position).XY()
                .ScaleTo(seekerProjectile.maxTrackingVelocity);

            currentVelocity = currentVelocity.Lerp(targetDirection, seekerProjectile.trackingLerpFactor);

            rigidbody2D.velocity = currentVelocity;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            if (target == null)
            {
                Gizmos.DrawWireCube(transform.position, trackingSize);
            }
        }
    }
}
