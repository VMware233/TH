#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VMFramework.Core;
using VMFramework.GameLogicArchitecture;
using VMFramework.GameLogicArchitecture.Editor;

namespace TH.Map
{
    public partial class RoomGeneralSetting
    {
        protected override void OnHandleRegisterGameItemPrefabFromSelectedObject(
            Object obj, bool checkUnique)
        {
            if (obj is not RoomAsset roomAsset)
            {
                return;
            }

            if (checkUnique)
            {
                if (GamePrefabManager.GetAllGamePrefabs<Room>().Any(prefab => prefab.roomAsset == roomAsset))
                {
                    return;
                }
            }

            GamePrefabWrapperCreator.CreateGamePrefabWrapper(new Room()
            {
                id = roomAsset.name.ToSnakeCase(),
                roomAsset = roomAsset,
                initialGameTypesID = roomAsset.gameTypes.ToList()
            });
        }

        protected override void OnHandleUnregisterGameItemPrefabFromSelectedObject(Object obj)
        {
            if (obj is not RoomAsset roomAsset)
            {
                return;
            }

            GamePrefabWrapperRemover.RemoveGamePrefabWrapperWhere<Room>(room => room.roomAsset == roomAsset);
        }

        protected override bool ShowRegisterGameItemPrefabFromSelectionButton(
            IEnumerable<Object> objects)
        {
            return objects.Any(obj => obj is RoomAsset);
        }
    }
}
#endif