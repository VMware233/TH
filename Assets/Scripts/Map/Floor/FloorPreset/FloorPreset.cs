using System;
using VMFramework.GameLogicArchitecture;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using TH.GameCore;
using TH.Items;
using UnityEngine;
using VMFramework.Configuration;
using VMFramework.Core;
using VMFramework.ExtendedTilemap;
using VMFramework.GameEvents;
using VMFramework.OdinExtensions;

namespace TH.Map
{
    public partial class FloorPreset : DescribedGamePrefab
    {
        protected override string idSuffix => "floor";

        public override Type gameItemType => typeof(Floor);

        protected virtual Type floorControllerType => null;

        [LabelText("预热数量"), TabGroup(TAB_GROUP_NAME, BASIC_SETTING_CATEGORY)]
        [MinValue(0), MaxValue(1000)]
        public int prewarmCount = 10;

        [LabelText("瓦片ID"), TabGroup(TAB_GROUP_NAME, BASIC_SETTING_CATEGORY)]
        [GamePrefabID(typeof(ExtendedRuleTile))]
        [JsonProperty]
        public string tileID;

        [LabelText("使用网格复合碰撞器"), TabGroup(TAB_GROUP_NAME, BASIC_SETTING_CATEGORY)]
        public bool useGridCompositeCollider = true;

        [LabelText("使用盒碰撞器"), TabGroup(TAB_GROUP_NAME, BASIC_SETTING_CATEGORY)]
        [HideIf(nameof(useGridCompositeCollider))]
        public bool useBoxCollider = true;

        [LabelText("盒碰撞器尺寸"), TabGroup(TAB_GROUP_NAME, BASIC_SETTING_CATEGORY)]
        [IsNotNullOrEmpty]
        [HideIf(nameof(useGridCompositeCollider))]
        [EnableIf(nameof(useBoxCollider))]
        public RectangleFloatConfig boxColliderSize = new(Vector2.zero, Vector2.one);

        [LabelText("刚体"), TabGroup(TAB_GROUP_NAME, BASIC_SETTING_CATEGORY)]
        [HideIf(nameof(useGridCompositeCollider))]
        public bool isRigidbody = true;

        [LabelText("是否启用鼠标碰撞器"), TabGroup(TAB_GROUP_NAME, BASIC_SETTING_CATEGORY)]
        public bool enableMouseCollider;

        [LabelText("掉落物品"), TabGroup(TAB_GROUP_NAME, BASIC_SETTING_CATEGORY)]
        public IChooserConfig<List<ItemGenerationConfig>> dropItems;

        public override void CheckSettings()
        {
            base.CheckSettings();
            
            dropItems?.CheckSettings();
        }

        protected override void OnInit()
        {
            base.OnInit();
            
            dropItems?.Init();
        }

        public virtual FloorController CreateController()
        {
            var floorGameObject = new GameObject(name)
            {
                layer = GameSetting.floorGeneralSetting.floorLayer
            };

            floorGameObject.transform.ResetGlobalArguments();

            FloorController floorController;
            if (floorControllerType != null)
            {
                floorController = floorGameObject.AddComponent(floorControllerType) as FloorController;
            }
            else
            {
                floorController = floorGameObject.AddComponent<FloorController>();
            }
            
            floorController.AssertIsNotNull(nameof(floorController));

            if (useGridCompositeCollider == false)
            {
                if (isRigidbody)
                {
                    var rigidbody2d = floorController.GetOrAddComponent<Rigidbody2D>();

                    rigidbody2d.bodyType = RigidbodyType2D.Static;
                }

                if (useBoxCollider)
                {
                    var boxCollider = floorController.GetOrAddComponent<BoxCollider2D>();

                    var boxColliderRectangle = new RectangleFloat(boxColliderSize) *
                                               GameSetting.floorGeneralSetting.cellSize;

                    boxCollider.size = boxColliderRectangle.size;
                    boxCollider.offset = boxColliderRectangle.pivot;

                    boxCollider.isTrigger = isRigidbody == false;
                }
            }

            if (enableMouseCollider)
            {
                var collider = floorController.graphicTransform.GetOrAddComponent<BoxCollider>();
                collider.size = GameSetting.floorGeneralSetting.cellSize.InsertAsZ(1);
                collider.center = collider.size / 2;
                var trigger = floorController.graphicTransform.GetOrAddComponent<ColliderMouseEventTrigger>();
                if (trigger.owner == null)
                {
                    trigger.SetOwner(floorController.transform);
                }
            }

            return floorController;
        }
    }
}
