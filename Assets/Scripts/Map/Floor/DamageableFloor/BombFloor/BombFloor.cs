using Cysharp.Threading.Tasks;
using TH.Damage;
using UnityEngine;
using VMFramework.Core;

namespace TH.Map
{
    public class BombFloor : DamageableFloor, IDamageSource
    {
        protected BombFloorPreset bombFloorPreset => (BombFloorPreset)gamePrefab;

        protected override async void OnDestroy(FloorDestructionInfo info)
        {
            base.OnDestroy(info);

            if (isServer)
            {
                var raycastOrigin = tilePivotRealPos;

                await UniTask.NextFrame();

                foreach (var direction in bombFloorPreset.explosionRaycastCount
                             .GetUniformlySpacedDirectionsOfCircle())
                {
                    var hit = Physics2D.Raycast(raycastOrigin, direction, bombFloorPreset.explosionRadius,
                        bombFloorPreset.explosionRaycastLayerMask);

                    if (hit && hit.TryGetDamageable(out var damageable))
                    {
                        this.RequestTakeDamage(damageable);
                    }
                }
            }
        }

        void IDamageSource.ProduceDamagePacket(IDamageable target, out DamagePacket packet)
        {
            packet = new DamagePacket(this)
            {
                isMelee = true,
                physicalDamage = bombFloorPreset.explosionDamage,
            };
        }
    }
}
