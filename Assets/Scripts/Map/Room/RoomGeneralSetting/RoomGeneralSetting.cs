using System;
using System.Collections.Generic;
using System.Linq;
using VMFramework.GameLogicArchitecture;
using VMFramework.Core;
using EnumsNET;
using Sirenix.OdinInspector;
using UnityEngine;
using VMFramework.OdinExtensions;

namespace TH.Map
{
    public sealed partial class RoomGeneralSetting : GamePrefabGeneralSetting
    {
        #region Categories

        private const string ROOM_FLAG_CATEGORY = "房间标记";

        private const string ROOM_IMPORT_CATEGORY = "房间导入";
        
        private const string ROOM_CATEGORY = "房间设置";

        #endregion

        #region Flags

        [LabelText("房间出入口标记"), TabGroup(TAB_GROUP_NAME, ROOM_FLAG_CATEGORY)]
        [ShowInInspector, EnableGUI, DisplayAsString]
        public const string ROOM_GATEWAY_FLAG = "GATEWAY";

        #endregion

        #region Metadata
        
        public override Type baseGamePrefabType => typeof(Room);

        #endregion

        [LabelText("房间单元尺寸"), TabGroup(TAB_GROUP_NAME, ROOM_CATEGORY)]
        public Vector2Int roomUnitSize;

        [ShowInInspector, TabGroup(TAB_GROUP_NAME, DEBUGGING_CATEGORY)]
        private Dictionary<FourTypesDirection2D, Dictionary<string, HashSet<Room>>>
            roomDictByDirection;

        protected override void OnPostInit()
        {
            base.OnPostInit();

            roomDictByDirection = new();

            foreach (var direction in FourTypesDirection2D.All.GetFlags())
            {
                roomDictByDirection[direction] = new();
            }

            foreach (var room in GamePrefabManager.GetAllGamePrefabs<Room>())
            {
                foreach (var gatewayInfo in room.gatewayInfos)
                {
                    var roomDictByGameType =
                        roomDictByDirection[gatewayInfo.gatewayDirection];

                    foreach (var gameTypeID in room.gameTypeSet.gameTypesID)
                    {
                        var gameType = GameType.GetGameType(gameTypeID);

                        foreach (var gameTypeLeaf in gameType.GetAllLeaves(true))
                        {
                            if (roomDictByGameType.ContainsKey(gameTypeLeaf.id) == false)
                            {
                                roomDictByGameType[gameTypeLeaf.id] = new();
                            }

                            roomDictByGameType[gameTypeLeaf.id].Add(room);
                        }

                        //Debug.Log(
                        //    $"{room.id}|{gameType.id}:" +
                        //    $"{gameType.GetAllLeaves(true).Select(gameTypeLeaf => gameTypeLeaf.id).ToString(",")}");
                    }
                }
            }
        }

        [Button, TabGroup(TAB_GROUP_NAME, DEBUGGING_CATEGORY)]
        public IEnumerable<Room> GetRoomsWithoutGateway()
        {
            return GamePrefabManager.GetAllGamePrefabs<Room>().Where(room => room.gatewayInfos.Count == 0)
                .ToList();
        }

        [Button, TabGroup(TAB_GROUP_NAME, DEBUGGING_CATEGORY)]
        public IReadOnlyCollection<Room> GetRoomsHavingGatewayDirection(
            FourTypesDirection2D direction,
            [GameType] string roomGameType)
        {
            var result = new HashSet<Room>();

            var leavesGameTypes =
                GameType.GetGameType(roomGameType).GetAllLeaves(true).ToList();

            var roomDictByGameType = roomDictByDirection[direction];

            foreach (var leafGameType in leavesGameTypes)
            {
                if (roomDictByGameType.TryGetValue(leafGameType, out var rooms))
                {
                    result.UnionWith(rooms);
                }
            }

            return result;
        }

        [Button, TabGroup(TAB_GROUP_NAME, DEBUGGING_CATEGORY)]
        public IReadOnlyCollection<Room> GetRoomsHavingAnyGatewayDirection(
            FourTypesDirection2D directions,
            [GameType] string roomGameType)
        {
            var result = new HashSet<Room>();

            foreach (var direction in directions.GetFlags())
            {
                result.UnionWith(
                    GetRoomsHavingGatewayDirection(direction, roomGameType));
            }

            return result;
        }

        [Button, TabGroup(TAB_GROUP_NAME, DEBUGGING_CATEGORY)]
        public IReadOnlyCollection<Room> GetRoomsHavingAllGatewayDirection(
            FourTypesDirection2D directions,
            [GameType] string roomGameType)
        {
            var result = new HashSet<Room>();

            foreach (var direction in directions.GetFlags())
            {
                var rooms = GetRoomsHavingGatewayDirection(direction, roomGameType);

                if (result.Count == 0)
                {
                    result.UnionWith(rooms);
                }
                else
                {
                    result.IntersectWith(rooms);
                }
            }

            return result;
        }
    }
}
