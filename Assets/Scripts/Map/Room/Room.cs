using VMFramework.GameLogicArchitecture;
using System.Collections.Generic;
using System.Linq;
using VMFramework.Core;
using EnumsNET;
using Sirenix.OdinInspector;
using TH.GameCore;
using UnityEngine;

namespace TH.Map
{
    public class Room : GameTypedGamePrefab
    {
        [LabelText("房间资源"), TabGroup(TAB_GROUP_NAME, BASIC_SETTING_CATEGORY)]
        [Required]
        public RoomAsset roomAsset;

        [LabelText("房间出入口信息"), TabGroup(TAB_GROUP_NAME, BASIC_SETTING_CATEGORY)]
        public IReadOnlyList<RoomGatewayInfo> gatewayInfos;

        public IReadOnlyCollection<Vector2Int> validRelativeUnitPoses =>
            roomAsset.validUnitPos;

        [HideInInspector]
        public int gatewayCount => gatewayInfos.Count;

        public IEnumerable<(Vector2Int relativePos, string floorID)> GetFloorIDs()
        {
            foreach (var unitPos in validRelativeUnitPoses)
            {
                foreach (var (pos, floorID) in roomAsset.GetFloorsIDByUnitPos(unitPos))
                {
                    yield return (pos, floorID);
                }
            }
        }

        [Button("获取尺寸"), TabGroup(TAB_GROUP_NAME, TOOLS_CATEGORY)]
        public Vector2Int GetSize()
        {
            return roomAsset.floorsID.GetSize();
        }

        #region Init & Check

        public override bool isPreInitializationRequired => true;

        public override bool isInitializationRequired => false;

        public override bool isPostInitializationRequired => false;

        public override bool isInitializationCompleteRequired => false;

        public override void CheckSettings()
        {
            base.CheckSettings();

            foreach (var gameTypeID in roomAsset.gameTypes)
            {
                if (gameTypeID.IsNullOrEmpty())
                {
                    continue;
                }

                if (GameType.HasGameType(gameTypeID))
                {
                    if (gameTypeSet.HasGameType(gameTypeID) == false)
                    {
                        gameTypeSet.AddGameType(gameTypeID);
                    }
                }
            }

            foreach (var gameTypeID in gameTypeSet.leafGameTypes.ToList())
            {
                if (initialGameTypesID.Count <= 1)
                {
                    break;
                }

                if (roomAsset.gameTypes.Contains(gameTypeID) == false)
                {
                    gameTypeSet.RemoveGameType(gameTypeID);
                }
            }

            if (GetSize().AnyNumberBelowOrEqual(0))
            {
                Debug.LogWarning($"房间的尺寸{GetSize()}无效");
            }
        }

        protected override void OnPreInit()
        {
            base.OnPreInit();

            gatewayInfos = GenerateGatewayInfos();
        }

        #endregion

        public IEnumerable<RoomGatewayInfo> GetGatewayInfosByDirections(
            FourTypesDirection2D directions)
        {
            return gatewayInfos.Where(info => directions.HasFlag(info.gatewayDirection));
        }

        public bool TryGetGatewayInfosByDirections(FourTypesDirection2D directions,
            out IReadOnlyList<RoomGatewayInfo> infos)
        {
            infos = GetGatewayInfosByDirections(directions).ToList();

            return infos.Count > 0;
        }

        public bool HasAnyGatewayDirections(FourTypesDirection2D directions)
        {
            return gatewayInfos.Any(info => directions.HasFlag(info.gatewayDirection));
        }

        public bool HasAllGatewayDirections(FourTypesDirection2D directions)
        {
            foreach (var direction in directions.GetFlags())
            {
                if (gatewayInfos.Any(info => info.gatewayDirection == direction) ==
                    false)
                {
                    return false;
                }
            }

            return true;
        }

        [Button, TabGroup(TAB_GROUP_NAME, TOOLS_CATEGORY)]
        private IReadOnlyList<RoomGatewayInfo> GenerateGatewayInfos()
        {
            var roomSize = GetSize();

            var roomUnitSize = GameSetting.roomGeneralSetting.roomUnitSize;

            var allInfos = new List<RoomGatewayInfo>();

            var validRelativeUnitPoses = this.validRelativeUnitPoses;

            foreach (var unitPos in validRelativeUnitPoses)
            {
                var gatewayInfoByDirection =
                    new Dictionary<FourTypesDirection2D, List<Vector2Int>>();

                var unitRectangle = roomAsset.GetUnitRectangle(unitPos);

                var gatewayPoses = roomAsset.roomGatewayPoses
                    .Where(pos => unitRectangle.Contains(pos))
                    .ToList();

                foreach (var gatewayPos in gatewayPoses)
                {
                    if (unitRectangle.IsOnBoundary(gatewayPos,
                            out var boundaryDirections))
                    {
                        foreach (var direction in boundaryDirections.GetFlags())
                        {
                            var nearUnitPos = unitPos +
                                              direction.ToCardinalVector();

                            if (validRelativeUnitPoses.Contains(nearUnitPos))
                            {
                                continue;
                            }

                            if (gatewayInfoByDirection.ContainsKey(direction) == false)
                            {
                                gatewayInfoByDirection.Add(direction, new List<Vector2Int>());
                            }

                            gatewayInfoByDirection[direction].Add(gatewayPos);
                        }
                    }
                }

                foreach (var (direction, poses) in gatewayInfoByDirection)
                {
                    var indices = new HashSet<int>();

                    if (direction.IsHorizontal())
                    {
                        indices.UnionWith(poses.Select(pos => pos.y % roomUnitSize.y));
                    }
                    else if (direction.IsVertical())
                    {
                        indices.UnionWith(poses.Select(pos => pos.x % roomUnitSize.x));
                    }

                    allInfos.Add(new RoomGatewayInfo()
                    {
                        relativeUnitPos = unitPos,
                        gatewayDirection = direction,
                        poses = poses,
                        indices = indices
                    });
                }
            }

            return allInfos;
        }
    }
}
