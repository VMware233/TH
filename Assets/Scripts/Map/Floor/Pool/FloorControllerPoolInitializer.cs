using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;
using VMFramework.GameLogicArchitecture;
using VMFramework.Procedure;

namespace TH.Map
{
    [GameInitializerRegister(typeof(GameInitializationProcedure))][Preserve]
    public class FloorControllerPoolInitializer : IGameInitializer
    {
        public async void OnInitComplete(Action onDone)
        {
            Debug.Log("正在预热FloorController池");

            foreach (var floorPrefab in GamePrefabManager.GetAllGamePrefabs<FloorPreset>())
            {
                FloorControllerPool.Prewarm(floorPrefab.id, floorPrefab.prewarmCount);
                await UniTask.NextFrame();
            }

            onDone();
        }
    }
}