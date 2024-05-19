using System;
using Sirenix.OdinInspector;
using UnityEngine;
using VMFramework.Procedure;

namespace TH.Map
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(GameMap))]
    public class WorldGenerationTestMap : SerializedMonoBehaviour, IManagerBehaviour
    {
        [Required]
        public GameMap gameMap;

        [LabelText("房间生成信息")]
        [ShowInInspector]
        public WorldGenerator.WorldGenerationInfo info;

        private bool isInterrupted = false;

        #region Init

        void IInitializer.OnPreInit(Action onDone)
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

        #endregion

        private void Reset()
        {
            gameMap = GetComponent<GameMap>();
        }

        [Button("生成地图")]
        public void GenerateMap(
            [ValueDropdown("@GameSetting.worldGenerationRuleGeneralSetting.GetPrefabNameList()")]
            string ruleID)
        {
            info = WorldGenerator.GenerateMap(ruleID);
        }

        [Button("渲染所有房间")]
        public void RenderAllRooms()
        {
            gameMap.ClearMap();

            WorldGenerator.RenderRooms(gameMap, info);
        }

        [Button("渲染其他房间")]
        public void RenderOtherRooms()
        {
            gameMap.ClearMap();

            WorldGenerator.RenderRoomList(gameMap, info.otherRoomInfos);
        }

        [Button("渲染全部流程")]
        public void RenderAllProcedure()
        {
            gameMap.ClearMap();

            WorldGenerator.RenderRoomTree(gameMap, info.rootRoomInfo,
                new WorldGenerator.RoomTreeRenderHint()
                {
                    renderTarget = WorldGenerator.RoomTreeRenderHint.RenderTarget.All
                });
        }

        [Button("渲染分支流程")]
        public void RenderBranchProcedure()
        {
            gameMap.ClearMap();

            WorldGenerator.RenderRoomTree(gameMap, info.rootRoomInfo,
                new WorldGenerator.RoomTreeRenderHint()
                {
                    renderTarget = WorldGenerator.RoomTreeRenderHint.RenderTarget
                        .BranchProcedure
                });
        }

        [Button("渲染主流程（立刻）")]
        public void RenderMainProcedureInstantly()
        {
            gameMap.ClearMap();

            WorldGenerator.RenderRoomTree(gameMap, info.rootRoomInfo,
                new WorldGenerator.RoomTreeRenderHint()
                {
                    renderTarget = WorldGenerator.RoomTreeRenderHint.RenderTarget
                        .MainProcedure
                });
        }

        [Button("渲染所有流程（时间间隔）")]
        public async void RenderAllProceduresWithTimeGap(float timeGap = 0.1f)
        {
            gameMap.ClearMap();

            await WorldGenerator.RenderRoomTreeWithTimeGap(gameMap,
                info.rootRoomInfo, timeGap, new WorldGenerator.RoomTreeRenderHint()
                {
                    renderTarget = WorldGenerator.RoomTreeRenderHint.RenderTarget
                        .MainProcedure
                });

            foreach (var branchRootRoomInfo in info.branchRootRoomInfos)
            {
                await WorldGenerator.RenderRoomTreeWithTimeGap(gameMap, branchRootRoomInfo,
                    timeGap, new WorldGenerator.RoomTreeRenderHint()
                    {
                        renderTarget = WorldGenerator.RoomTreeRenderHint.RenderTarget
                            .BranchProcedure
                    });
            }
        }

        [Button("渲染主流程（时间间隔）")]
        public async void RenderMainProcedureWithTimeGap(float timeGap = 0.1f)
        {
            gameMap.ClearMap();

            await WorldGenerator.RenderRoomTreeWithTimeGap(gameMap,
                info.rootRoomInfo, timeGap, new WorldGenerator.RoomTreeRenderHint()
                {
                    renderTarget = WorldGenerator.RoomTreeRenderHint.RenderTarget
                        .MainProcedure
                });
        }

        [Button("生成主流程（中断）")]
        public void RenderMainProcedureWithInterruption()
        {
            gameMap.ClearMap();

            isInterrupted = false;

            WorldGenerator.RenderRoomTreeWithInterruption(gameMap, info.rootRoomInfo,
                () => isInterrupted, () => isInterrupted = true);
        }

        [Button("继续生成")]
        public void ContinueGeneration()
        {
            isInterrupted = false;
        }
    }
}
