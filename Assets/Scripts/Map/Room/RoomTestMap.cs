using System;
using Sirenix.OdinInspector;
using UnityEngine;
using VMFramework.Procedure;

namespace TH.Map
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(GameMap))]
    public class RoomTestMap : SerializedMonoBehaviour, IManagerBehaviour
    {
        [Required]
        public GameMap gameMap;

        private void Reset()
        {
            gameMap = GetComponent<GameMap>();
        }

        [Button("渲染房间")]
        public void RenderRoom(
            [ValueDropdown("@GameSetting.roomGeneralSetting.GetPrefabNameList()")]
            string roomID)
        {
            gameMap.ClearMap();

            gameMap.SetRoom(roomID, Vector2Int.zero, true);
        }

        public void OnPreInit(Action onDone)
        {
            ProcedureManager.AddOnEnterEvent(procedureID =>
            {
                if (procedureID == MainMenuProcedure.ID)
                {
                    gameMap.Init();
                }
            });

            onDone();
        }
    }
}
