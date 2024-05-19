using System;
using Sirenix.OdinInspector;
using UnityEngine;
using VMFramework.Core;

namespace TH.Map
{
    public class BombFloorPreset : DamageableFloorPreset
    {
        public override Type gameItemType => typeof(BombFloor);

        protected const string BOMB_CATEGORY = "炸弹设置";

        [LabelText("爆炸半径"), TabGroup(TAB_GROUP_NAME, BOMB_CATEGORY)]
        [MinValue(0)]
        public int explosionRadius = 1;

        [LabelText("爆炸伤害"), TabGroup(TAB_GROUP_NAME, BOMB_CATEGORY)]
        [MinValue(0)]
        public int explosionDamage = 1;

        [LabelText("爆炸射线层"), TabGroup(TAB_GROUP_NAME, BOMB_CATEGORY)]
        public LayerMask explosionRaycastLayerMask;

        [LabelText("爆炸射线数量"), TabGroup(TAB_GROUP_NAME, BOMB_CATEGORY)]
        [MinValue(1)]
        public int explosionRaycastCount = 8;

        public override void CheckSettings()
        {
            base.CheckSettings();

            explosionRaycastCount.AssertIsAboveOrEqual(1, nameof(explosionRaycastCount));
        }
    }
}