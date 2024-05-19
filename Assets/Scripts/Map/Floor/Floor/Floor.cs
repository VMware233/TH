using VMFramework.GameLogicArchitecture;
using System.Collections.Generic;
using UnityEngine;
using VMFramework.UI;
using TH.Entities;
using TH.GameCore;
using VMFramework.Map;

namespace TH.Map
{
    public partial class Floor : VisualGameItem, IContextMenuProvider, ITracingTooltipProvider
    {
        protected FloorPreset floorPreset => (FloorPreset)gamePrefab;
        
        public GameMap gameMap { get; private set; }
        public ITileBasicData tile { get; private set; }

        public Transform transform => floorController.transform;
        public Transform graphicTransform => floorController.graphicTransform;

        public Vector2 realPos => gameMap.GetRealPositionByTilePos(tile.xy);

        public Vector2 tilePivotRealPos => realPos + GameSetting.floorGeneralSetting.cellSize / 2;

        public GameMapNetwork gameMapNetwork => gameMap.GetComponent<GameMapNetwork>();

        public string tileID => floorPreset.tileID;

        public FloorController floorController { get; private set; }

        public Collider2D collider2D { get; protected set; }

        #region Init

        protected override void OnCreate()
        {
            base.OnCreate();

            isDestroyed = false;
        }

        public void Init(GameMap gameMap, ITileBasicData tile, FloorController floorController)
        {
            this.gameMap = gameMap;
            this.tile = tile;
            this.floorController = floorController;

            OnInit();

            if (floorController != null)
            {
                floorController.Init(this);
            }
        }

        protected virtual void OnInit()
        {
            if (floorPreset.useGridCompositeCollider)
            {
                collider2D = gameMap.AddGridCollision(this);
            }
            else
            {
                collider2D = floorController.GetComponent<Collider2D>();
            }

            collider2D.excludeLayers = GameSetting.floorGeneralSetting.floorLayer.ToLayerMask();
        }

        #endregion

        #region Destroy
        
        public bool isDestroyed { get; private set; } = false;

        public void Destroy(FloorDestructionInfo info)
        {
            OnDestroy(info);

            gameMap = null;
            floorController = null;

            isDestroyed = true;
        }

        protected virtual void OnDestroy(FloorDestructionInfo info)
        {
            if (floorPreset.useGridCompositeCollider)
            {
                gameMap.RemoveGridCollision(tile.xy);
            }

            if (info.enableDroppings && isServer)
            {
                if (floorPreset.dropItems != null)
                {
                    var configs = floorPreset.dropItems.GetValue();
                    foreach (var itemGenerationConfig in configs)
                    {
                        var item = itemGenerationConfig.GenerateItem();
                        
                        if (item == null)
                        {
                            continue;
                        }
                        
                        EntityManager.CreateItemDrop(item, realPos);
                    }
                }
            }
        }

        #endregion

        #region Update

        public virtual void OnNearFloorUpdate()
        {

        }

        #endregion

        #region Context Menu Provider

        public IEnumerable<IContextMenuProvider.ContextMenuEntryConfig> GetContextMenuContent()
        {
            yield return new IContextMenuProvider.ContextMenuEntryConfig()
            {
                title = "破坏",
                action = () => gameMap.DestroyFloor(tile.xy, new FloorDestructionInfo()
                {
                    enableDroppings = true
                })
            };
        }

        #endregion

        #region To String

        protected override IEnumerable<(string propertyID, string propertyContent)> OnGetStringProperties()
        {
            if (tile == null)
            {
                yield break;
            }
            yield return ("pos", tile.xy.ToString());
        }

        #endregion
    }
}