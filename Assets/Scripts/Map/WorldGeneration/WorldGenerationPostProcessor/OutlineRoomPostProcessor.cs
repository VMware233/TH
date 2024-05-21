using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using VMFramework.Core;
using VMFramework.GameLogicArchitecture;
using VMFramework.OdinExtensions;

namespace TH.Map
{
    [InfoBox("描边房间")]
    public class OutlineRoomPostProcessor : WorldGenerationPostProcessor
    {
        [LabelText("房间游戏类型")]
        [IsNotNullOrEmpty]
        [GameTypeID]
        public string roomGameTypeID;

        [LabelText("轮廓宽度")]
        public int outlineWidth = 1;

        [LabelText("非空房间所占权重")]
        public int nonEmptyRoomWeight = 1;

        public override void Process(WorldGenerator.WorldGenerationInfo info)
        {
            var outlineUnitPoses = new HashSet<Vector2Int>();

            foreach (var existedUnitPos in info.existedUnitPos.Keys)
            {
                var (outlineMin, outlineMax) =
                    existedUnitPos.GetMinMaxPointFromPivotExtents(outlineWidth);

                foreach (var outlineUnitPos in outlineMin.GetAllPointsOfRectangle(outlineMax))
                {
                    outlineUnitPoses.Add(outlineUnitPos);
                }
            }

            outlineUnitPoses.ExceptWith(info.existedUnitPos.Keys);

            while (outlineUnitPoses.Count > 0)
            {
                var chosenUnitPos = outlineUnitPoses.Choose();

                var validRooms = GamePrefabManager.GetGamePrefabsByGameType<Room>(roomGameTypeID).ToList();

                validRooms.RemoveAll(room => room.validRelativeUnitPoses.Any(relativePos =>
                    info.existedUnitPos.ContainsKey(chosenUnitPos + relativePos)));

                if (validRooms.Count == 0)
                {
                    Debug.LogWarning("无法填充剩余空间");
                    break;
                }

                var weights = new int[validRooms.Count];

                for (var i = 0; i < validRooms.Count; i++)
                {
                    var room = validRooms[i];

                    int nonEmptyCount = 0;
                    foreach (var (relativePos, floorID) in room.GetFloorIDs())
                    {
                        if (floorID.IsNullOrEmpty())
                        {
                            continue;
                        }

                        nonEmptyCount++;
                    }

                    weights[i] += nonEmptyCount * nonEmptyRoomWeight;
                }

                var chosenRoom = validRooms.Choose(weights);

                var newRoomGenerationInfo =
                    new WorldGenerator.RoomGenerationInfo(chosenRoom, chosenUnitPos,
                        false);

                foreach (var unitPos in newRoomGenerationInfo.GetAllUnitPoses())
                {
                    info.existedUnitPos[unitPos] = newRoomGenerationInfo;
                    outlineUnitPoses.Remove(unitPos);
                }

                info.otherRoomInfos.Add(newRoomGenerationInfo);
            }
        }
    }
}