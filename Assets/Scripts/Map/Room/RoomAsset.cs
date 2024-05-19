using System.Collections.Generic;
using VMFramework.Core;
using Sirenix.OdinInspector;
using TH.GameCore;
using UnityEngine;
using VMFramework.GameLogicArchitecture;

namespace TH.Map
{
    public struct RoomRawInfo
    {
        public List<string>[,] rawArray;
        public List<string> gameTypes;
    }

    public class RoomAsset : SerializedScriptableObject
    {
        public Vector2Int size
        {
            get => _size;
            private set => _size = value;
        }

        public string[,] floorsID
        {
            get => _floorsID;
            private set => _floorsID = value;
        }

        public IReadOnlyCollection<Vector2Int> roomGatewayPoses => _roomGatewayPoses;

        [LabelText("地板ID")]
        [SerializeField]
        private string[,] _floorsID;

        [LabelText("游戏种类")]
        [SerializeField]
        private List<string> _gameTypes;

        [LabelText("尺寸")]
        [SerializeField]
        private Vector2Int _size;

        [LabelText("房间开口坐标")]
        [SerializeField]
        private HashSet<Vector2Int> _roomGatewayPoses = new();

        [LabelText("有效区块单元")]
        [SerializeField]
        private HashSet<Vector2Int> _validUnitPos = new();

        public IReadOnlyCollection<Vector2Int> validUnitPos => _validUnitPos;

        public List<string> gameTypes => _gameTypes;

        public void Load(RoomRawInfo roomRawInfo)
        {
            var rawArray = roomRawInfo.rawArray;

            _gameTypes = roomRawInfo.gameTypes;

            size = rawArray.GetSize();

            var width = size.x;
            var height = size.y;

            floorsID = new string[width, height];
            _roomGatewayPoses = new();
            _validUnitPos = new();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var allTileData = rawArray[x, y];

                    if (allTileData == null)
                    {
                        continue;
                    }

                    foreach (var tileData in allTileData)
                    {
                        if (tileData == RoomGeneralSetting.ROOM_GATEWAY_FLAG)
                        {
                            _roomGatewayPoses.Add(new(x, y));
                            continue;
                        }

                        if (GamePrefabManager.ContainsGamePrefab(tileData))
                        {
                            floorsID[x, y] = tileData;
                            continue;
                        }

                        if (GameSetting.roomGeneralSetting.enableIDAndFlagImportCheckingWarning)
                        {
                            Debug.LogWarning($"房间{name}中" +
                                             $"({x},{y})单元的" +
                                             $"数据:{tileData}既不是地板ID，也不是标记");
                        }
                    }
                }
            }

            if (width % GameSetting.roomGeneralSetting.roomUnitSize.x != 0)
            {
                Debug.LogWarning($"房间{name}的宽度{width}不是" +
                                 $"房间单元尺寸{GameSetting.roomGeneralSetting.roomUnitSize.x}的整数倍");
            }

            if (height % GameSetting.roomGeneralSetting.roomUnitSize.y != 0)
            {
                Debug.LogWarning($"房间{name}的高度{height}不是" +
                                 $"房间单元尺寸{GameSetting.roomGeneralSetting.roomUnitSize.y}的整数倍");
            }

            var maxUnitXPos = width / GameSetting.roomGeneralSetting.roomUnitSize.x;
            var maxUnitYPos = height / GameSetting.roomGeneralSetting.roomUnitSize.y;

            for (int x = 0; x < maxUnitXPos; x++)
            {
                for (int y = 0; y < maxUnitYPos; y++)
                {
                    int validFloorCount = 0;
                    foreach (var (_, floorID) in GetFloorsIDByUnitPos(new(x, y)))
                    {
                        if (floorID.IsNullOrEmpty() == false)
                        {
                            _validUnitPos.Add(new(x, y));
                            validFloorCount++;
                        }
                    }

                    if (validFloorCount == 0)
                    {
                        continue;
                    }

                    if (validFloorCount.F() /
                        (GameSetting.roomGeneralSetting.roomUnitSize.Products()) < 0.03f)
                    {
                        Debug.LogWarning(
                            $"{name}房间资源的区块单元:{(x, y)}" +
                            $"有效地板数量:{validFloorCount}过少，请检查是否是误画");
                    }
                }
            }
        }

        public RectangleInteger GetUnitRectangle(Vector2Int unitPos)
        {
            var startPos = unitPos.Multiply(GameSetting.roomGeneralSetting.roomUnitSize);

            var endPos = startPos + GameSetting.roomGeneralSetting.roomUnitSize
                         - Vector2Int.one;

            return new(startPos, endPos);
        }

        public IEnumerable<(Vector2Int pos, string floorID)> GetFloorsIDByUnitPos(
            Vector2Int unitPos)
        {
            var rect = GetUnitRectangle(unitPos);

            return floorsID.EnumerateRectangle(rect.min, rect.max);
        }
    }
}
