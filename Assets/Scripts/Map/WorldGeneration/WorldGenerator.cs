using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using TH.GameCore;
using UnityEngine;
using VMFramework.Core;
using VMFramework.Core.Linq;
using VMFramework.GameLogicArchitecture;

namespace TH.Map
{
    public static class WorldGenerator
    {
        public class WorldGenerationInfo
        {
            public WorldGenerationRule rule;

            public RoomGenerationInfo rootRoomInfo;

            public List<RoomGenerationInfo> branchRootRoomInfos = new();

            public Dictionary<Vector2Int, RoomGenerationInfo> existedUnitPos = new();

            public List<RoomGenerationInfo> otherRoomInfos = new();

            public void AddNewGenerationInfo(RoomGenerationInfo newGenerationInfo)
            {
                foreach (var unitPos in newGenerationInfo.GetAllUnitPoses())
                {
                    if (existedUnitPos.TryAdd(unitPos, newGenerationInfo) == false)
                    {
                        Debug.LogWarning($"覆盖已有坐标：{unitPos}失败，" + $"旧生成信息:{existedUnitPos[unitPos]}，" +
                                         $"试图覆盖成:{newGenerationInfo}");
                    }
                }
            }
        }

        public class RoomGenerationInfo : ITreeNode<RoomGenerationInfo>
        {
            private readonly RoomGenerationInfo parentRoomInfo;

            private readonly Dictionary<RoomGatewayInfo, RoomGenerationInfo> _childrenRoomDict = new();

            [ShowInInspector]
            private List<RoomGatewayInfo> gatewayInfos =>
                _childrenRoomDict?.Keys.ToList();

            [ShowInInspector]
            public Room room { get; init; }

            [ShowInInspector]
            public RoomGatewayInfo enterGatewayInfo { get; init; }

            /// <summary>
            ///     房间左下角在地图上的区块坐标
            /// </summary>
            [ShowInInspector]
            public Vector2Int startUnitPos { get; init; }

            [ShowInInspector]
            public bool isMainProcedure { get; init; }

            [ShowInInspector]
            public int depth { get; private set; }

            public Room parentRoom => parentRoomInfo?.room;

            public IReadOnlyDictionary<RoomGatewayInfo, RoomGenerationInfo> childrenRoomDict =>
                _childrenRoomDict;

            public RoomGenerationInfo(Room room, Vector2Int startUnitPos, bool isMainProcedure)
            {
                this.room = room;
                this.startUnitPos = startUnitPos;
                this.isMainProcedure = isMainProcedure;
                depth = 0;
            }

            public RoomGenerationInfo(RoomGenerationInfo parentRoomInfo,
                RoomGatewayInfo parentExitGatewayInfo, RoomGatewayInfo enterGatewayInfo, Room room,
                Vector2Int startUnitPos, bool isMainProcedure)
            {
                this.room = room;
                this.startUnitPos = startUnitPos;

                this.enterGatewayInfo = enterGatewayInfo;
                this.parentRoomInfo = parentRoomInfo;
                parentRoomInfo._childrenRoomDict.Add(parentExitGatewayInfo, this);

                this.isMainProcedure = isMainProcedure;

                depth = parentRoomInfo.depth + 1;
            }

            public IEnumerable<Vector2Int> GetAllUnitPoses()
            {
                foreach (var relativeUnitPos in room.validRelativeUnitPoses)
                {
                    yield return startUnitPos + relativeUnitPos;
                }
            }

            public Vector2Int GetGatewayNearUnitPos(RoomGatewayInfo gatewayInfo)
            {
                gatewayInfo.AssertIsNotNull(nameof(gatewayInfo));

                if (room.gatewayInfos.Contains(gatewayInfo) == false)
                {
                    throw new ArgumentException($"房间:{room}不包含出口:{gatewayInfo}");
                }

                return startUnitPos + gatewayInfo.relativeUnitPos +
                       gatewayInfo.gatewayDirection.ToCardinalVector();
            }

            #region ITreeNode

            RoomGenerationInfo IParentProvider<RoomGenerationInfo>.GetParent()
            {
                return parentRoomInfo;
            }

            [ShowInInspector]
            IEnumerable<RoomGenerationInfo> IChildrenProvider<RoomGenerationInfo>.GetChildren()
            {
                return _childrenRoomDict.Values.ToList();
            }

            #endregion
        }

        private class NextRoomInfo
        {
            public Room nextRoom;

            public Vector2Int startUnitPos;

            public RoomGatewayInfo enterGatewayInfo;

            public HashSet<RoomGatewayInfo> validExitGatewayInfos;

            public Vector2Int GetGatewayNearUnitPos(RoomGatewayInfo gatewayInfo)
            {
                gatewayInfo.AssertIsNotNull(nameof(gatewayInfo));

                if (nextRoom.gatewayInfos.Contains(gatewayInfo) == false)
                {
                    throw new ArgumentException($"房间:{nextRoom}不包含出口:{gatewayInfo}");
                }

                return startUnitPos + gatewayInfo.relativeUnitPos +
                       gatewayInfo.gatewayDirection.ToCardinalVector();
            }
        }

        public static FastNoise noise = new();

        public static WorldGenerationInfo GenerateMap(string ruleID)
        {
            return GenerateMap(GamePrefabManager.GetGamePrefabStrictly<WorldGenerationRule>(ruleID));
        }

        public static WorldGenerationInfo GenerateMap(WorldGenerationRule rule)
        {
            var worldGenerationInfo = new WorldGenerationInfo
            {
                rule = rule
            };

            Debug.Log("开始生成主流程");
            GenerateMainProcedureRoomTree(worldGenerationInfo);

            Debug.Log("开始生成分支流程");
            GenerateBranchProcedureRoomTree(worldGenerationInfo);

            Debug.Log("开始地图生成后处理");
            foreach (var postProcessor in rule.postProcessors)
            {
                postProcessor.Process(worldGenerationInfo);
            }

            return worldGenerationInfo;
        }

        #region Main Procedure

        private static void GenerateMainProcedureRoomTree(WorldGenerationInfo worldInfo)
        {
            var firstRoomGameTypeID = worldInfo.rule.firstRoomInfo.GetValue().roomGameTypeID;
            var room = GameSetting.roomGeneralSetting.GetRoomsHavingAnyGatewayDirection(
                    worldInfo.rule.firstRoomValidExitGatewayDirection, firstRoomGameTypeID)
                .Where(room => room.gatewayCount > 0)
                .Choose();

            var rootRoomInfo = new RoomGenerationInfo(room, Vector2Int.zero, true);

            worldInfo.AddNewGenerationInfo(rootRoomInfo);

            var procedureRootRoomInfo = rootRoomInfo;
            foreach (var (procedureIndex, roomProcedureChooser) in
                     worldInfo.rule.mainRoomProcedures.Enumerate())
            {
                var isLastProcedure = procedureIndex == worldInfo.rule.mainRoomProcedures.Count - 1;

                var roomProcedure = roomProcedureChooser.GetValue();

                var hint = new ProcedureGenerationHint
                {
                    isMainProcedure = true,
                    isLastProcedure = isLastProcedure,
                    enablePriorityDirectionOverride = false
                };

                GenerateProcedureRoomTree(procedureRootRoomInfo, roomProcedure, worldInfo, hint,
                    out procedureRootRoomInfo);
            }

            worldInfo.rootRoomInfo = rootRoomInfo;
        }

        #endregion

        #region Branch Procedure

        public static void GenerateBranchProcedureRoomTree(WorldGenerationInfo worldInfo)
        {
            var mainBranchMaxDepth = worldInfo.rootRoomInfo.PreorderTraverse(true).Select(info =>
            {
                if (info.isMainProcedure)
                {
                    return info.depth;
                }

                return -1;
            }).Max();

            foreach (var procedureChooser in worldInfo.rule.branchRoomProcedures)
            {
                var branchProcedure = procedureChooser.GetValue();

                var highestScore = int.MinValue;
                List<(RoomGenerationInfo roomGenerationInfo, RoomGatewayInfo gatewayInfo )>
                    highestScoreRoomExit = new();

                var globalMaxDepth = worldInfo.rootRoomInfo.PreorderTraverse(true).Select(info => info.depth)
                    .Max();

                foreach (var (roomGenerationInfo, gatewayInfo) in GetAllValidExitGatewayInfos(worldInfo))
                {
                    if (roomGenerationInfo.depth < worldInfo.rule.branchMinDepth)
                    {
                        continue;
                    }

                    if (worldInfo.rule.branchOnlyOnMainBranch)
                    {
                        if (roomGenerationInfo.isMainProcedure == false)
                        {
                            continue;
                        }

                        if (mainBranchMaxDepth - roomGenerationInfo.depth <
                            worldInfo.rule.branchMaxMainBranchDepthDistance)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (globalMaxDepth - roomGenerationInfo.depth <
                            worldInfo.rule.branchMaxGlobalDepthDistance)
                        {
                            continue;
                        }
                    }

                    var score = EvaluateBranchExitGateway(worldInfo, branchProcedure, roomGenerationInfo,
                        gatewayInfo);

                    if (score > highestScore)
                    {
                        highestScore = score;
                        highestScoreRoomExit.Clear();
                    }

                    if (score == highestScore)
                    {
                        highestScoreRoomExit.Add((roomGenerationInfo, gatewayInfo));
                    }
                }

                if (highestScoreRoomExit.Count == 0)
                {
                    Debug.LogWarning("没有找到有效出口，提前终止生成");
                    break;
                }

                var (highestScoreRoomInfo, highestScoreExitGatewayInfo) = highestScoreRoomExit.Choose();

                var hint = new ProcedureGenerationHint
                {
                    isMainProcedure = false,
                    isLastProcedure = true,
                    enablePriorityDirectionOverride = true,
                    overridePriorityDirection = highestScoreExitGatewayInfo.gatewayDirection
                };

                GenerateProcedureRoomTree(highestScoreRoomInfo, branchProcedure, worldInfo, hint, out _);

                worldInfo.branchRootRoomInfos.Add(highestScoreRoomInfo);
            }
        }

        private static int EvaluateBranchExitGateway(WorldGenerationInfo worldInfo,
            RoomGenerationProcedure procedure, RoomGenerationInfo roomGenerationInfo,
            RoomGatewayInfo exitGatewayInfo)
        {
            var score = 0;

            var exitGatewayNearUnitPos = roomGenerationInfo.GetGatewayNearUnitPos(exitGatewayInfo);

            var extents = worldInfo.rule.branchEvaluationRangeSize / 2;
            var minPos = exitGatewayNearUnitPos - extents;
            var maxPos = exitGatewayNearUnitPos + extents;
            var unitPosRect = new RectangleInteger(minPos, maxPos);

            var emptyCount = 0;

            var traversedUnitPoses = new HashSet<Vector2Int>();

            foreach (var unitPos in exitGatewayNearUnitPos.LevelOrderTraverse(false, NearUnitPointsGetter))
            {
                emptyCount++;

                traversedUnitPoses.Add(unitPos);
            }

            score += emptyCount * worldInfo.rule.branchRectEmptyWeight;

            if (worldInfo.branchRootRoomInfos.Count != 0)
            {
                var minDepthDistanceToOtherBranch = int.MaxValue;

                foreach (var branchRootRoomInfo in worldInfo.branchRootRoomInfos)
                {
                    var depthDistance = (roomGenerationInfo.depth - branchRootRoomInfo.depth).Abs();

                    if (depthDistance < minDepthDistanceToOtherBranch)
                    {
                        minDepthDistanceToOtherBranch = depthDistance;
                    }
                }

                score += minDepthDistanceToOtherBranch *
                         worldInfo.rule.branchMinDepthDistanceToOtherBranchWeight;
            }

            return score;

            IEnumerable<Vector2Int> NearUnitPointsGetter(Vector2Int unitPos)
            {
                foreach (var nearUnitPos in unitPos.GetFourDirectionsNearPoints())
                {
                    if (worldInfo.existedUnitPos.ContainsKey(nearUnitPos))
                    {
                        continue;
                    }

                    if (unitPosRect.Contains(nearUnitPos) == false)
                    {
                        continue;
                    }

                    if (traversedUnitPoses.Contains(nearUnitPos))
                    {
                        continue;
                    }

                    yield return nearUnitPos;
                }
            }
        }

        private static IEnumerable<(RoomGenerationInfo roomGenerationInfo, RoomGatewayInfo gatewayInfo)>
            GetAllValidExitGatewayInfos(WorldGenerationInfo worldInfo)
        {
            foreach (var roomGenerationInfo in worldInfo.rootRoomInfo.PreorderTraverse(true))
            {
                var validExitGatewayInfos = roomGenerationInfo.room.gatewayInfos.ToList();

                validExitGatewayInfos.Remove(roomGenerationInfo.enterGatewayInfo);

                foreach (var gatewayInfo in roomGenerationInfo.childrenRoomDict.Keys)
                {
                    validExitGatewayInfos.Remove(gatewayInfo);
                }

                foreach (var gatewayInfo in validExitGatewayInfos)
                {
                    if (worldInfo.existedUnitPos.ContainsKey(
                            roomGenerationInfo.GetGatewayNearUnitPos(gatewayInfo)))
                    {
                        continue;
                    }

                    yield return (roomGenerationInfo, gatewayInfo);
                }
            }
        }

        #endregion

        #region Generate Procedure

        private struct ProcedureGenerationHint
        {
            public bool isMainProcedure;
            public bool isLastProcedure;
            public bool enablePriorityDirectionOverride;
            public FourTypesDirection2D overridePriorityDirection;
        }

        private static void GenerateProcedureRoomTree(RoomGenerationInfo rootRoomInfo,
            RoomGenerationProcedure roomProcedure, WorldGenerationInfo worldInfo,
            ProcedureGenerationHint hint, out RoomGenerationInfo lastRoomInfo)
        {
            rootRoomInfo.AssertIsNotNull(nameof(rootRoomInfo));

            var currentRoomInfo = rootRoomInfo;

            var priorityDirection = roomProcedure.priorityDirection;

            if (hint.enablePriorityDirectionOverride)
            {
                hint.overridePriorityDirection.AssertIsSingleFlag(nameof(hint.overridePriorityDirection));

                priorityDirection = hint.overridePriorityDirection;
            }

            var reversedPriorityDirection = priorityDirection.Reversed();

            var validRoomExitGatewayDirections = priorityDirection.Reversed().GetOtherDirections();

            foreach (var (unitIndex, procedureUnitChooser) in roomProcedure.units.Enumerate())
            {
                var isLastUnit = unitIndex == roomProcedure.units.Count - 1;

                var procedureUnit = procedureUnitChooser.GetValue();

                if (rootRoomInfo == null)
                {
                    var room = GameSetting.roomGeneralSetting
                        .GetRoomsHavingAnyGatewayDirection(validRoomExitGatewayDirections,
                            procedureUnit.roomGameTypeID).Where(room => room.gatewayCount > 0).Choose();

                    rootRoomInfo = new RoomGenerationInfo(room, Vector2Int.zero, true);

                    SetCurrentGenerationInfo(rootRoomInfo);

                    continue;
                }

                var validCurrentRoomExitGatewayInfos = currentRoomInfo.room.gatewayInfos.ToList();

                validCurrentRoomExitGatewayInfos.Remove(currentRoomInfo.enterGatewayInfo);

                validCurrentRoomExitGatewayInfos.RemoveAll(gatewayInfo =>
                    gatewayInfo.gatewayDirection == reversedPriorityDirection);

                if (validCurrentRoomExitGatewayInfos.Count == 0)
                {
                    throw new Exception("未知错误导致当前房间无法继续扩展");
                }

                //剔除被堵住的出口
                foreach (var validExitGatewayInfo in validCurrentRoomExitGatewayInfos.ToArray())
                {
                    var exitGatewayUnitPos = currentRoomInfo.startUnitPos +
                                             validExitGatewayInfo.relativeUnitPos;

                    var nearPos = exitGatewayUnitPos +
                                  validExitGatewayInfo.gatewayDirection.ToCardinalVector();

                    if (worldInfo.existedUnitPos.ContainsKey(nearPos))
                    {
                        validCurrentRoomExitGatewayInfos.Remove(validExitGatewayInfo);
                    }
                }

                if (validCurrentRoomExitGatewayInfos.Count == 0)
                {
                    Debug.LogWarning("房间被堵死，提前终止生成");

                    lastRoomInfo = currentRoomInfo;
                    return;
                }

                var exitGatewayWeights = new int[validCurrentRoomExitGatewayInfos.Count];

                for (var i = 0; i < validCurrentRoomExitGatewayInfos.Count; i++)
                {
                    var validCurrentRoomExitGatewayInfo = validCurrentRoomExitGatewayInfos[i];

                    var isPriorityDirection =
                        validCurrentRoomExitGatewayInfo.gatewayDirection == priorityDirection;

                    var isPriorityDirectionWeight = isPriorityDirection
                        ? roomProcedure.isPriorityDirectionExitGatewayChooseWeight
                        : 0;

                    var isConsecutiveDirectionWeight = 0;

                    if (currentRoomInfo.enterGatewayInfo != null)
                    {
                        var consecutiveDirectionCount = 0;

                        foreach (var roomGenerationInfo in currentRoomInfo.TraverseToRoot(true))
                        {
                            if (roomGenerationInfo.enterGatewayInfo == null)
                            {
                                break;
                            }

                            if (roomGenerationInfo.enterGatewayInfo.gatewayDirection ==
                                validCurrentRoomExitGatewayInfo.gatewayDirection.Reversed() == false)
                            {
                                break;
                            }

                            consecutiveDirectionCount++;
                        }

                        var isConsecutiveDirection = currentRoomInfo.enterGatewayInfo.gatewayDirection ==
                                                     validCurrentRoomExitGatewayInfo.gatewayDirection
                                                         .Reversed();

                        isConsecutiveDirectionWeight = isConsecutiveDirection
                            ? roomProcedure.isConsecutiveDirectionExitGatewayChooseWeight *
                              consecutiveDirectionCount
                            : 0;
                    }

                    exitGatewayWeights[i] = isPriorityDirectionWeight + isConsecutiveDirectionWeight;
                }

                PostprocessWeights(exitGatewayWeights);

                var exitGatewayInfo = validCurrentRoomExitGatewayInfos.Choose(exitGatewayWeights);

                var exitGatewayDirection = exitGatewayInfo.gatewayDirection;

                var nextRoomEnterGatewayDirection = exitGatewayDirection.Reversed();

                //当前房间的出口所在的区块坐标
                var currentExitGatewayUnitPos = currentRoomInfo.startUnitPos +
                                                exitGatewayInfo.relativeUnitPos;

                //下一个房间的入口所在的区块坐标
                var nextRoomEnterGatewayUnitPos =
                    currentExitGatewayUnitPos + exitGatewayDirection.ToCardinalVector();

                //筛选出包含入口方向的特定种类房间
                var tempValidNextRoomList = GameSetting.roomGeneralSetting
                    .GetRoomsHavingGatewayDirection(nextRoomEnterGatewayDirection,
                        procedureUnit.roomGameTypeID).ToList();

                if (tempValidNextRoomList.Count == 0)
                {
                    throw new Exception($"没有找到种类为:{procedureUnit.roomGameTypeID}且" +
                                        $"包含入口方向{nextRoomEnterGatewayDirection}的房间");
                }

                //进一步筛选出包含下一个房间该有的有效出口方向（由流程的优先方向决定）的房间
                tempValidNextRoomList = tempValidNextRoomList.Where(room =>
                    room.HasAnyGatewayDirections(validRoomExitGatewayDirections)).ToList();

                if (tempValidNextRoomList.Count == 0)
                {
                    throw new Exception($"没有找到包含出口方向:{validRoomExitGatewayDirections}的房间");
                }

                var validNextRoomDict = new Dictionary<Room, List<NextRoomInfo>>();

                //进一步筛选出不会与已有房间重叠的房间
                foreach (var validNextRoom in tempValidNextRoomList)
                {
                    var validGateways = validNextRoom
                        .GetGatewayInfosByDirections(nextRoomEnterGatewayDirection).ToList();

                    if (validGateways.Count == 0)
                    {
                        throw new Exception("未知错误");
                    }

                    foreach (var gateway in validGateways)
                    {
                        var roomStartPos = nextRoomEnterGatewayUnitPos - gateway.relativeUnitPos;

                        var validUnitPoses = validNextRoom.validRelativeUnitPoses
                            .Select(pos => pos + roomStartPos).ToList();

                        //如果有任何一个区块坐标已经被占用，则跳过
                        if (validUnitPoses.Any(worldInfo.existedUnitPos.ContainsKey))
                        {
                            continue;
                        }

                        if (validNextRoomDict.ContainsKey(validNextRoom) == false)
                        {
                            validNextRoomDict.Add(validNextRoom, new List<NextRoomInfo>());
                        }

                        validNextRoomDict[validNextRoom].Add(new NextRoomInfo
                        {
                            nextRoom = validNextRoom,
                            enterGatewayInfo = gateway,
                            startUnitPos = roomStartPos
                        });
                    }
                }

                //进一步筛选掉所有有效方向的出口都被堵住的房间
                //除非是最后一个流程的最后一个单元，否则不允许出现这种情况
                var pendingRemovalRoomDict = new Dictionary<Room, List<NextRoomInfo>>();
                foreach (var (validNextRoom, nextRoomInfoList) in validNextRoomDict)
                {
                    foreach (var nextRoomInfo in nextRoomInfoList)
                    {
                        var exitGatewayInfos = validNextRoom
                            .GetGatewayInfosByDirections(validRoomExitGatewayDirections).ToList();

                        exitGatewayInfos.Remove(nextRoomInfo.enterGatewayInfo);

                        nextRoomInfo.validExitGatewayInfos = new();

                        if (exitGatewayInfos.Count != 0)
                        {
                            foreach (var exitGateway in exitGatewayInfos)
                            {
                                var nearPos = nextRoomInfo.GetGatewayNearUnitPos(exitGateway);

                                if (worldInfo.existedUnitPos.ContainsKey(nearPos) == false)
                                {
                                    nextRoomInfo.validExitGatewayInfos.Add(exitGateway);
                                }
                            }
                        }

                        if (hint.isLastProcedure && isLastUnit)
                        {
                            continue;
                        }

                        if (nextRoomInfo.validExitGatewayInfos.Count == 0)
                        {
                            if (pendingRemovalRoomDict.ContainsKey(validNextRoom) == false)
                            {
                                pendingRemovalRoomDict.Add(validNextRoom, new List<NextRoomInfo>());
                            }

                            pendingRemovalRoomDict[validNextRoom].Add(nextRoomInfo);
                        }
                    }
                }

                RemoveInvalidNextRooms();

                if (validNextRoomDict.Count == 0)
                {
                    Debug.LogWarning("没有找到有效房间提前终止生成");

                    lastRoomInfo = currentRoomInfo;
                    return;
                }

                //进一步筛选掉所有有效方向的出口与当前房间的出口失配率过高的房间
                if (worldInfo.rule.gatewayMismatchRate != 0)
                {
                    pendingRemovalRoomDict.Clear();
                    foreach (var (validNextRoom, nextRoomInfoList) in validNextRoomDict)
                    {
                        foreach (var nextRoomInfo in nextRoomInfoList)
                        {
                            var enterGatewayInfo = nextRoomInfo.enterGatewayInfo;

                            var overlapIndices = enterGatewayInfo.indices.ToHashSet();

                            overlapIndices.IntersectWith(exitGatewayInfo.indices);

                            var overlapCount = overlapIndices.Count;

                            var enterGatewayMismatchRate =
                                (enterGatewayInfo.indices.Count - overlapCount).F() /
                                enterGatewayInfo.indices.Count;

                            var exitGatewayMismatchRate = (exitGatewayInfo.indices.Count - overlapCount).F() /
                                                          exitGatewayInfo.indices.Count;

                            var mismatchRate = enterGatewayMismatchRate.Min(exitGatewayMismatchRate);

                            if (mismatchRate >= worldInfo.rule.gatewayMismatchRate)
                            {
                                if (pendingRemovalRoomDict.ContainsKey(validNextRoom) == false)
                                {
                                    pendingRemovalRoomDict.Add(validNextRoom, new List<NextRoomInfo>());
                                }

                                pendingRemovalRoomDict[validNextRoom].Add(nextRoomInfo);
                            }
                        }
                    }

                    RemoveInvalidNextRooms();

                    if (validNextRoomDict.Count == 0)
                    {
                        Debug.LogWarning("没有找到有效房间提前终止生成");

                        lastRoomInfo = currentRoomInfo;
                        return;
                    }
                }

                var nextRoomInfos = new List<NextRoomInfo>();

                foreach (var nextRoomInfoList in validNextRoomDict.Values)
                {
                    nextRoomInfos.AddRange(nextRoomInfoList);
                }

                var nextRoomWeights = new int[nextRoomInfos.Count];

                for (var i = 0; i < nextRoomInfos.Count; i++)
                {
                    var nextRoomInfo = nextRoomInfos[i];

                    var validExitGatewayCount = nextRoomInfo.validExitGatewayInfos.Count;

                    var validExitGatewayCountWeight = validExitGatewayCount *
                                                      roomProcedure.validExitGatewayCountNextRoomChooseWeight;

                    var hasPriorityDirection = nextRoomInfo.validExitGatewayInfos.Any(gatewayInfo =>
                        gatewayInfo.gatewayDirection == priorityDirection);

                    var hasPriorityDirectionWeight = hasPriorityDirection
                        ? roomProcedure.includingPriorityDirectionNextRoomChooseWeight
                        : 0;

                    var consecutiveDirectionCount = 1;

                    foreach (var roomGenerationInfo in currentRoomInfo.TraverseToRoot(true))
                    {
                        if (roomGenerationInfo.enterGatewayInfo == null)
                        {
                            break;
                        }

                        if (roomGenerationInfo.enterGatewayInfo.gatewayDirection !=
                            nextRoomEnterGatewayDirection)
                        {
                            break;
                        }

                        consecutiveDirectionCount++;
                    }

                    var hasConsecutiveDirection = nextRoomInfo.validExitGatewayInfos.Any(gatewayInfo =>
                        gatewayInfo.gatewayDirection == nextRoomEnterGatewayDirection);

                    var includingConsecutiveDirectionWeight = 0;
                    var excludingConsecutiveDirectionWeight = 0;
                    if (hasConsecutiveDirection)
                    {
                        includingConsecutiveDirectionWeight = consecutiveDirectionCount *
                                                              roomProcedure
                                                                  .includingConsecutiveDirectionNextRoomChooseWeight;
                    }
                    else
                    {
                        excludingConsecutiveDirectionWeight = consecutiveDirectionCount *
                                                              roomProcedure
                                                                  .excludingConsecutiveDirectionNextRoomChooseWeight;
                    }

                    var validUnitCount = nextRoomInfo.nextRoom.validRelativeUnitPoses.Count;
                    var validUnitCountWeight = validUnitCount *
                                               roomProcedure.validUnitCountNextRoomChooseWeight;

                    nextRoomWeights[i] = validExitGatewayCountWeight + hasPriorityDirectionWeight +
                                         includingConsecutiveDirectionWeight +
                                         excludingConsecutiveDirectionWeight + validUnitCountWeight;
                }

                PostprocessWeights(nextRoomWeights);

                var finalNextRoomInfo = nextRoomInfos.Choose(nextRoomWeights);

                var nextRoomGenerationInfo = new RoomGenerationInfo(currentRoomInfo, exitGatewayInfo,
                    finalNextRoomInfo.enterGatewayInfo, finalNextRoomInfo.nextRoom,
                    finalNextRoomInfo.startUnitPos, hint.isMainProcedure);

                SetCurrentGenerationInfo(nextRoomGenerationInfo);

                continue;

                void PostprocessWeights(int[] weights)
                {
                    var minWeight = weights.Min();

                    if (minWeight < 0)
                    {
                        for (var i = 0; i < weights.Length; i++)
                        {
                            weights[i] -= minWeight;
                        }
                    }

                    var maxWeight = weights.Max();
                    if (maxWeight <= 0)
                    {
                        for (var i = 0; i < weights.Length; i++)
                        {
                            weights[i] += 1;
                        }
                    }
                }

                void RemoveInvalidNextRooms()
                {
                    foreach (var (pendingRemovalRoom, pendingRemovalNextRoomInfoList) in
                             pendingRemovalRoomDict)
                    {
                        foreach (var nextRoomInfo in pendingRemovalNextRoomInfoList)
                        {
                            validNextRoomDict[pendingRemovalRoom].Remove(nextRoomInfo);
                        }

                        if (validNextRoomDict[pendingRemovalRoom].Count == 0)
                        {
                            validNextRoomDict.Remove(pendingRemovalRoom);
                        }
                    }
                }
            }

            lastRoomInfo = currentRoomInfo;

            void SetCurrentGenerationInfo(RoomGenerationInfo newGenerationInfo)
            {
                currentRoomInfo = newGenerationInfo;

                worldInfo.AddNewGenerationInfo(currentRoomInfo);
            }
        }

        #endregion

        #region Render Room

        public struct RoomTreeRenderHint
        {
            [Flags]
            public enum RenderTarget
            {
                MainProcedure = 1,
                BranchProcedure = 2,
                All = MainProcedure | BranchProcedure
            }

            public RenderTarget renderTarget;
        }

        public static void RenderRooms(GameMap gameMap, WorldGenerationInfo worldGenerationInfo)
        {
            RenderRoomTree(gameMap, worldGenerationInfo.rootRoomInfo, new RoomTreeRenderHint
            {
                renderTarget = RoomTreeRenderHint.RenderTarget.All
            });
            RenderRoomList(gameMap, worldGenerationInfo.otherRoomInfos);
        }

        public static void RenderRoomTree(GameMap gameMap, RoomGenerationInfo rootRoomGenerationInfo,
            RoomTreeRenderHint hint)
        {
            foreach (var roomGenerationInfo in rootRoomGenerationInfo.PreorderTraverse(true))
            {
                if (roomGenerationInfo.isMainProcedure)
                {
                    if (hint.renderTarget.HasFlag(RoomTreeRenderHint.RenderTarget.MainProcedure) == false)
                    {
                        continue;
                    }
                }
                else
                {
                    if (hint.renderTarget.HasFlag(RoomTreeRenderHint.RenderTarget.BranchProcedure) == false)
                    {
                        continue;
                    }
                }

                var startPos =
                    roomGenerationInfo.startUnitPos.Multiply(GameSetting.roomGeneralSetting.roomUnitSize);
                gameMap.SetRoom(roomGenerationInfo.room.id, startPos, true);
            }
        }

        public static void RenderRoomList(GameMap gameMap,
            IEnumerable<RoomGenerationInfo> roomGenerationInfos)
        {
            foreach (var roomGenerationInfo in roomGenerationInfos)
            {
                var startPos =
                    roomGenerationInfo.startUnitPos.Multiply(GameSetting.roomGeneralSetting.roomUnitSize);
                gameMap.SetRoom(roomGenerationInfo.room.id, startPos, true);
            }
        }

        public static async UniTask RenderRoomTreeWithTimeGap(GameMap gameMap,
            RoomGenerationInfo rootRoomGenerationInfo, float timeGap, RoomTreeRenderHint hint)
        {
            foreach (var roomGenerationInfo in rootRoomGenerationInfo.PreorderTraverse(true))
            {
                if (roomGenerationInfo.isMainProcedure)
                {
                    if (hint.renderTarget.HasFlag(RoomTreeRenderHint.RenderTarget.MainProcedure) == false)
                    {
                        continue;
                    }
                }
                else
                {
                    if (hint.renderTarget.HasFlag(RoomTreeRenderHint.RenderTarget.BranchProcedure) == false)
                    {
                        continue;
                    }
                }

                var startPos =
                    roomGenerationInfo.startUnitPos.Multiply(GameSetting.roomGeneralSetting.roomUnitSize);
                gameMap.SetRoom(roomGenerationInfo.room.id, startPos, true);

                await UniTask.Delay(TimeSpan.FromSeconds(timeGap));
            }
        }

        public static async void RenderRoomTreeWithInterruption(GameMap gameMap,
            RoomGenerationInfo rootRoomGenerationInfo, Func<bool> isInterrupted, Action interruptAction)
        {
            foreach (var roomGenerationInfo in rootRoomGenerationInfo.PreorderTraverse(true))
            {
                var startPos =
                    roomGenerationInfo.startUnitPos.Multiply(GameSetting.roomGeneralSetting.roomUnitSize);
                gameMap.SetRoom(roomGenerationInfo.room.id, startPos, true);

                interruptAction();
                await UniTask.WaitUntil(() => isInterrupted() == false);
            }
        }

        #endregion
    }
}