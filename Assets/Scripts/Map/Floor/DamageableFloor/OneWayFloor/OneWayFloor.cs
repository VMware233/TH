using TH.Entities;
using TH.GameCore;
using VMFramework.Core;
using UnityEngine;

namespace TH.Map
{
    public class OneWayFloor : DamageableFloor, ICrouchActivatable
    {
        #region Init

        protected override void OnInit()
        {
            base.OnInit();

            var platformEffector2D = transform.GetOrAddComponent<PlatformEffector2D>();

            platformEffector2D.surfaceArc = 90;
            platformEffector2D.useColliderMask = false;

            collider2D.AssertIsNotNull(nameof(collider2D));

            collider2D.usedByEffector = true;
        }

        #endregion

        #region Update

        public override void OnNearFloorUpdate()
        {
            base.OnNearFloorUpdate();

            // bool hasNearFloor = false;
            //
            // if (gameMap.TryGetFloor(tile.xy + Vector2Int.left, out var leftFloor))
            // {
            //     if (leftFloor is not OneWayFloor)
            //     {
            //         hasNearFloor = true;
            //     }
            // }
            //
            // if (hasNearFloor)
            // {
            //     return;
            // }
            //
            // if (gameMap.TryGetFloor(tile.xy + Vector2Int.right, out var rightFloor))
            // {
            //     if (rightFloor is not OneWayFloor)
            //     {
            //         hasNearFloor = true;
            //     }
            // }
            //
            // if (hasNearFloor)
            // {
            //     return;
            // }
            //
            // gameMap.DestroyFloor(tile.xy, new FloorDestructionInfo()
            // {
            //     enableDroppings = true,
            // });
        }

        #endregion

        #region Crouch Activatable

        void ICrouchActivatable.CrouchActivate(Player player)
        {
            collider2D.excludeLayers =
                collider2D.excludeLayers.AddLayer(GameSetting.playerGeneralSetting.playerLayer);
        }

        void ICrouchActivatable.CrouchInActivate(Player player)
        {
            collider2D.excludeLayers =
                collider2D.excludeLayers.RemoveLayer(GameSetting.playerGeneralSetting.playerLayer);
        }

        #endregion
    }
}
