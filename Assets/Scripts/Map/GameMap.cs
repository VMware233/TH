using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using TH.GameCore;
using UnityEngine;
using VMFramework.Core;
using VMFramework.Core.Linq;
using VMFramework.ExtendedTilemap;
using VMFramework.GameLogicArchitecture;
using VMFramework.Map;
using VMFramework.OdinExtensions;

namespace TH.Map
{
    [DisallowMultipleComponent]
    public class GameMap : MonoBehaviour
    {
        #region MapCoreData

        public class Chunk : CubeMapCore<Chunk, Tile>.Chunk
        {
            public Transform chunkGroup;

            public CompositeCollider2D compositeCollider;

            public bool isRendered;

            protected override void OnClear()
            {
                Destroy(chunkGroup.gameObject);
            }
        }

        public class Tile : CubeMapCore<Chunk, Tile>.Tile
        {
            public Floor floor;

            public FloorController floorController;

            protected override void OnClear()
            {
                floor?.gameMap.ForceDestroyFloor(floor.tile.xy, new FloorDestructionInfo
                {
                    enableDroppings = false
                });
            }
        }

        #endregion

        private static WorldGeneralSetting worldSetting => GameSetting.worldGeneralSetting;

        private static FloorGeneralSetting floorSetting => GameSetting.floorGeneralSetting;

        public event Action<Chunk> OnChunkCreated;
        public event Action<Chunk> OnChunkDeleted;

        [Required]
        [SerializeField]
        private ExtendedTilemap floorTilemap;

        [LabelText("地图核心配置ID")]
        [GamePrefabID(typeof(MapCoreConfiguration))]
        public string mapCoreConfigID;

        [LabelText("地图核心")]
        [ShowInInspector]
        private CubeMapCore<Chunk, Tile>.Map mapCore;

        #region Init

        public void Init()
        {
            transform.parent = worldSetting.container;
            transform.ResetLocalArguments();

            floorTilemap.SetCellSize(GameSetting.floorGeneralSetting.cellSize);

            mapCore = new(mapCoreConfigID);

            mapCore.OnChunkCreateStart += chunk =>
            {
                var chunkGroup = new GameObject(chunk.pos.ToString())
                {
                    transform =
                    {
                        parent = transform,
                        position = GetRealPositionByTilePos(chunk.minTilePos.XY()),
                        localRotation = Quaternion.identity
                    },
                    layer = floorSetting.floorLayer
                }.transform;

                chunk.chunkGroup = chunkGroup;

                var rigidbody = chunkGroup.AddComponent<Rigidbody2D>();

                rigidbody.bodyType = RigidbodyType2D.Static;

                var compositeCollider = chunkGroup.AddComponent<CompositeCollider2D>();

                compositeCollider.geometryType = CompositeCollider2D.GeometryType.Polygons;

                compositeCollider.generationType = CompositeCollider2D.GenerationType.Synchronous;

                chunk.compositeCollider = compositeCollider;
            };

            mapCore.OnChunkCreateEnd += chunk => OnChunkCreated?.Invoke(chunk);
            mapCore.OnChunkDeleteEnd += chunk => OnChunkDeleted?.Invoke(chunk);
        }

        #endregion

        #region Update

        private void Update()
        {
            UpdateFloors();
        }

        #endregion

        #region CoreFloorFunc

        private void SetFloor(Vector2Int pos, Floor floor, bool replace)
        {
            if (floor == null)
            {
                if (replace)
                {
                    DestroyFloor(pos, new FloorDestructionInfo { enableDroppings = false });
                }

                return;
            }

            var tilePos = pos.As3DXY();
            var tile = mapCore.GetTileOrCreateChunk(tilePos);

            if (tile.floor != null && replace == false)
            {
                return;
            }

            tile.floor = floor;

            if (tile.floorController == null)
            {
                var floorController = FloorControllerPool.Get(floor.id);
                floorController.name = pos.ToString();
                floorController.transform.parent = tile.originChunk.chunkGroup;
                floorController.transform.position = GetRealPositionByTilePos(pos);

                tile.floorController = floorController;
            }

            floor.Init(this, tile, tile.floorController);

            UpdateNearFloors(pos);

            //floorTilemap.SetTile(pos, floor.tileID);
        }

        private void SetFloor(Vector2Int pos, string floorID, bool replace)
        {
            Floor floor = null;
            if (floorID.IsNullOrEmpty() == false)
            {
                floor = IGameItem.Create<Floor>(floorID);
            }

            SetFloor(pos, floor, replace);
        }

        private void ForceDestroyFloor(Vector2Int pos, FloorDestructionInfo info)
        {
            var tile = mapCore.GetTileOrCreateChunk(pos.As3DXY());

            if (tile.floor == null)
            {
                return;
            }

            var floor = tile.floor;

            FloorControllerPool.Return(floor.id, tile.floorController);

            floor.Destroy(info);

            UpdateNearFloors(pos);

            floorTilemap.ClearTile(pos);

            tile.floor = null;
            tile.floorController = null;
        }

        #endregion

        #region Floor

        #region Fill

        [Button("填补地板")]
        public void FillFloor(Vector2Int pos,
            [ValueDropdown("@GameSetting.floorGeneralSetting.GetPrefabNameList()")] string floorID)
        {
            SetFloor(pos, floorID, false);
        }

        [Button("填补矩形区域地板")]
        public void FillFloorInRectangleArea(Vector2Int start, Vector2Int end,
            [ValueDropdown("@GameSetting.floorGeneralSetting.GetPrefabNameList()")] string floorID)
        {
            start.GetAllPointsOfRectangle(end).Examine(pos => FillFloor(pos, floorID));
        }

        #endregion

        #region Replace

        [Button("替换地板")]
        public void ReplaceFloor(Vector2Int pos,
            [ValueDropdown("@GameSetting.floorGeneralSetting.GetPrefabNameList()")] string floorID)
        {
            SetFloor(pos, floorID, true);
        }

        [Button("替换矩形区域地板")]
        public void ReplaceFloorInRectangleArea(Vector2Int start, Vector2Int end,
            [ValueDropdown("@GameSetting.floorGeneralSetting.GetPrefabNameList()")] string floorID,
            bool renderInstantly)
        {
            start.GetAllPointsOfRectangle(end).Examine(pos => ReplaceFloor(pos, floorID));
        }

        #endregion

        #region Destroy

        [Button("销毁地板")]
        public void DestroyFloor(Vector2Int pos, FloorDestructionInfo info)
        {
            ForceDestroyFloor(pos, info);
        }

        #endregion

        #region Get

        public Floor GetFloor(Vector2Int pos)
        {
            var tile = mapCore.GetTile(pos.As3DXY());

            return tile?.floor;
        }

        public bool TryGetFloor(Vector2Int pos, out Floor floor)
        {
            floor = GetFloor(pos);

            return floor != null;
        }

        #endregion

        #region Get Near Floors

        public IEnumerable<Floor> GetNearFloors(Vector2Int pos)
        {
            foreach (var nearPoint in pos.GetFourDirectionsNearPoints())
            {
                var floor = GetFloor(nearPoint);

                if (floor != null)
                {
                    yield return floor;
                }
            }
        }

        #endregion

        #region Update

        private readonly HashSet<Vector2Int> floorUpdateList = new();

        public void UpdateNearFloors(Vector2Int pos)
        {
            floorUpdateList.AddRange(pos.GetFourDirectionsNearPoints());
        }

        public void UpdateFloors()
        {
            foreach (var pos in floorUpdateList)
            {
                GetFloor(pos)?.OnNearFloorUpdate();
            }
        }

        #endregion

        #endregion

        #region Chunk

        public IEnumerable<Vector2Int> GetAllChunksPos()
        {
            return mapCore.allChunks.Select(chunk => chunk.pos.XY());
        }

        public GameMapNetwork.ChunkLoadData GetChunkData(Vector2Int chunkPos)
        {
            if (mapCore.TryGetChunk(chunkPos.As3DXY(), out var chunk) == false)
            {
                chunk = mapCore.CreateChunk(chunkPos.As3DXY());
            }

            var data = new GameMapNetwork.ChunkLoadData
            {
                chunkPos = chunkPos,
                allFloorsData = new()
            };

            foreach (var tile in chunk)
            {
                data.allFloorsData[tile.xy] = tile.floor;
            }

            return data;
        }

        public void LoadChunkData(GameMapNetwork.ChunkLoadData data)
        {
            var chunk = mapCore.GetChunkByXY(data.chunkPos) ?? mapCore.CreateChunk(data.chunkPos.As3DXY());

            foreach (var (pos, floor) in data.allFloorsData)
            {
                SetFloor(pos, floor, true);
            }
        }

        public async void DynamicLoadingChunks(params Vector2Int[] startChunkPos)
        {
            mapCore.AddOriginChunkPos(startChunkPos.Select(pos => pos.As3DXY()).ToArray());
            await mapCore.DynamicLoadingChunks();
            mapCore.RemoveOriginChunkPos(startChunkPos.Select(pos => pos.As3DXY()).ToArray());
        }

        public async void DynamicDeleteChunks(params Vector2Int[] startChunkPos)
        {
            mapCore.AddOriginChunkPos(startChunkPos.Select(pos => pos.As3DXY()).ToArray());
            await mapCore.DynamicDeleteChunks();
            mapCore.RemoveOriginChunkPos(startChunkPos.Select(pos => pos.As3DXY()).ToArray());
        }

        //[Button("创建所有区块")]
        //public IReadOnlyList<Chunk> CreateAllChunks()
        //{
        //    return mapCore.CreateAllChunks();
        //}

        #endregion

        #region Clear

        [Button("清空地图")]
        public void ClearMap()
        {
            mapCore.Clear();
        }

        #endregion

        #region Render

        [Button]
        public void RenderChunk(Vector2Int pos)
        {
            if (mapCore.TryGetChunk(pos.As3DXY(), out var chunk) == false)
            {
                return;
            }

            if (chunk.isRendered)
            {
                return;
            }

            foreach (var tile in chunk)
            {
                if (tile.floor == null)
                {
                    continue;
                }

                floorTilemap.SetTile(tile.xy, tile.floor.tileID);
            }

            chunk.isRendered = true;
        }

        [Button]
        public async void RenderChunksFromPivot(Vector2Int pivot)
        {
            var radius = GameSetting.worldGeneralSetting.renderRadius;

            foreach (var chunkPos in pivot.GetPointsOfManhattanCircle(radius))
            {
                RenderChunk(chunkPos);

                await UniTask.NextFrame();
            }
        }

        #endregion

        #region PositionConvertion

        [Button("通过瓦片坐标获取真实坐标")]
        public Vector2 GetRealPositionByTilePos(Vector2Int tilePos)
        {
            return floorTilemap.GetRealPosition(tilePos);
        }

        [Button("通过真实坐标获取瓦片坐标")]
        public Vector2Int GetTilePosByRealPosition(Vector3 realPos)
        {
            return floorTilemap.GetTilePosition(realPos);
        }

        [Button("通过真实坐标获取区块坐标")]
        public Vector2Int GetChunkPosByRealPosition(Vector3 realPos)
        {
            return mapCore.GetChunkPosByTilePos(GetTilePosByRealPosition(realPos).As3DXY()).XY();
        }

        #endregion

        #region Collision

        [ShowInInspector]
        private Dictionary<Vector2Int, BoxCollider2D> gridColliders = new();

        [ShowInInspector]
        private Dictionary<BoxCollider2D, Vector2Int> gridCollidersReverse = new();

        public Collider2D AddGridCollision(Floor floor)
        {
            if (floor == null)
            {
                return null;
            }

            if (gridColliders.TryGetValue(floor.tile.xy, out var boxCollider2D))
            {
                boxCollider2D.enabled = true;
                return null;
            }

            var tile = mapCore.GetTile(floor.tile.pos);

            var chunkTransform = tile.originChunk.chunkGroup;

            boxCollider2D = chunkTransform.AddComponent<BoxCollider2D>();

            var size = GameSetting.floorGeneralSetting.cellSize;

            boxCollider2D.size = size;

            boxCollider2D.offset = GetRealPositionByTilePos(tile.xyInChunk) + size / 2;

            gridColliders.Add(floor.tile.xy, boxCollider2D);

            gridCollidersReverse.Add(boxCollider2D, floor.tile.xy);

            return boxCollider2D;
        }

        public void RemoveGridCollision(Vector2Int pos)
        {
            if (gridColliders.TryGetValue(pos, out var boxCollider2D))
            {
                boxCollider2D.enabled = false;

                gridColliders.Remove(pos);
            }
        }

        public bool TryGetFloorByGridCollider(BoxCollider2D boxCollider2D, out Floor floor)
        {
            if (gridCollidersReverse.TryGetValue(boxCollider2D, out var pos))
            {
                floor = GetFloor(pos);
                return true;
            }

            floor = null;
            return false;
        }

        #endregion

        #region Room

        [Button("渲染房间")]
        public void SetRoom(
            [ValueDropdown("@GameSetting.roomGeneralSetting.GetPrefabNameList()")] string roomID,
            Vector2Int startPos, bool replace)
        {
            var room = GamePrefabManager.GetGamePrefabStrictly<Room>(roomID);


            foreach (var (relativePos, floorID) in room.GetFloorIDs())
            {
                var pos = startPos + relativePos;

                if (replace)
                {
                    if (floorID.IsNullOrEmpty())
                    {
                        DestroyFloor(pos, new FloorDestructionInfo
                        {
                            enableDroppings = false
                        });
                    }
                    else
                    {
                        ReplaceFloor(pos, floorID);
                    }
                }
                else
                {
                    FillFloor(pos, floorID);
                }
            }
        }

        #endregion
    }
}