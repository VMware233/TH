using System;
using FishNet;
using FishNet.Object;
using UnityEngine;
using UnityEngine.Scripting;
using VMFramework.GameLogicArchitecture;
using VMFramework.Procedure;

namespace TH.Entities
{
    [GameInitializerRegister(typeof(ClientLoadingProcedure))]
    [Preserve]
    public class EntityControllerPoolClientInitializer : IGameInitializer
    {
        public void OnPostInit(Action onDone)
        {
            foreach (var entityConfig in GamePrefabManager.GetAllGamePrefabs<EntityConfig>())
            {
                var networkObject = entityConfig.prefab.GetComponent<NetworkObject>();
                if (networkObject == null)
                {
                    Debug.LogWarning($"实体:{entityConfig} 没有{nameof(NetworkObject)}组件");
                    continue;
                }

                InstanceFinder.NetworkManager.CacheObjects(networkObject, entityConfig.prewarmCount, false);
            }

            onDone();
        }
    }
}