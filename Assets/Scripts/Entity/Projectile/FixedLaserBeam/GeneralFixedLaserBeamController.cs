using Sirenix.OdinInspector;
using TH.Damage;
using UnityEngine;
using VMFramework.Core;
using VMFramework.ResourcesManagement;

namespace TH.Entities
{
    public sealed class GeneralFixedLaserBeamController : ProjectileController
    {
        #region Config

        [field: Required]
        [field: SerializeField]
        private GeneralLaserBeamGraphicController graphicController { get; set; }

        #endregion

        public GeneralFixedLaserBeam generalFixedLaserBeam => entity as GeneralFixedLaserBeam;

        private ParticleSystem impactParticle;

        protected override void OnInit()
        {
            base.OnInit();

            // Impact

            Vector2 direction = generalFixedLaserBeam.direction.normalized;

            graphicController.SetStart(transform.position);

            Vector2 endPoint;

            //TODO 以下用的是射线检测，要改为碰撞体
            var hit = Physics2D.Raycast(transform.position, direction, generalFixedLaserBeam.maxDistance,
                generalFixedLaserBeam.impactRaycastMask);

            if (hit)
            {
                endPoint = hit.point;

                impactParticle = ParticleSpawner.Spawn(generalFixedLaserBeam.impactParticleID, endPoint,
                    transform);

                var angle = Vector2.right.ClockwiseAngle(direction);

                impactParticle.transform.eulerAngles = new Vector3(0, 0, -angle + 180);
            }
            else
            {
                endPoint = transform.position.XY() + direction * generalFixedLaserBeam.maxDistance;
            }

            graphicController.SetEnd(endPoint);


            // Damage

            float distance = hit.distance + 0.2f;

            var damageHits = Physics2D.RaycastAll(transform.position, direction, distance,
                generalFixedLaserBeam.damageRaycastMask);

            foreach (var damageHit in damageHits)
            {
                if (damageHit.TryGetDamageable(out var damageable))
                {
                    if (damageable == generalFixedLaserBeam.sourceEntity)
                    {
                        continue;
                    }

                    generalFixedLaserBeam.RequestTakeDamage(damageable);
                }
            }
        }

        protected override void OnEntityControllerDisable()
        {
            base.OnEntityControllerDisable();

            ParticleSpawner.Return(impactParticle);
        }
    }
}
