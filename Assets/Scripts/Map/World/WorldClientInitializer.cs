using System;
using Cysharp.Threading.Tasks;
using FishNet;
using UnityEngine;
using UnityEngine.Scripting;
using VMFramework.Core;
using VMFramework.Procedure;

namespace TH.Map
{
    [GameInitializerRegister(typeof(ClientLoadingProcedure))]
    [Preserve]
    public sealed class WorldClientInitializer : IGameInitializer
    {
        async void IInitializer.OnPreInit(Action onDone)
        {
            await UniTask.WaitUntil(() =>
                WorldManager.currentWorldUUID.IsNullOrEmpty() == false);

            await UniTask.WaitUntil(() =>
                WorldManager.TryGetInfo(WorldManager.currentWorldUUID, out _));

            if (WorldManager.TryGetInfo(WorldManager.currentWorldUUID, out var info) ==
                false)
            {
                Debug.LogError($"无法找到默认世界：{WorldManager.currentWorldUUID}");
                return;
            }

            await UniTask.WaitUntil(() => info.owner.gameMapNetwork.initDone);

            if (InstanceFinder.ClientManager.Connection.IsHost == false)
            {
                await UniTask.WaitUntil(() => info.downloadComplete);
            }

            onDone();
        }
    }
}