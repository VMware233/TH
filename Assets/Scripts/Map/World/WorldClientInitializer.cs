using System;
using Cysharp.Threading.Tasks;
using FishNet;
using UnityEngine;
using UnityEngine.Scripting;
using VMFramework.Core;
using VMFramework.Network;
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
                UUIDCoreManager.TryGetInfo(WorldManager.currentWorldUUID, out _));

            if (UUIDCoreManager.TryGetOwnerWithWarning(WorldManager.currentWorldUUID, out World world) ==
                false)
            {
                return;
            }

            await UniTask.WaitUntil(() => world.gameMapNetwork.initDone);

            if (InstanceFinder.ClientManager.Connection.IsHost == false)
            {
                await UniTask.WaitUntil(() => WorldManager.IsDownloadCompleted(world));
            }

            onDone();
        }
    }
}