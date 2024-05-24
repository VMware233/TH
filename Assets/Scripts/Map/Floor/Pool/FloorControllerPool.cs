using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using VMFramework.Core;
using VMFramework.Core.Pool;
using VMFramework.GameLogicArchitecture;
using VMFramework.Procedure;

namespace TH.Map
{
    [ManagerCreationProvider(ManagerType.ResourcesCore)]
    public class FloorControllerPool : ManagerBehaviour<FloorControllerPool>
    {
        #region Config

        [LabelText("缓存容器")]
        [Required]
        [SerializeField]
        private Transform cachedTransformContainer;

        #endregion

        private static readonly Dictionary<string, IComponentPool<FloorController>> poolsDictionary = new();

        private static IComponentPool<FloorController> CreatePool(string floorID)
        {
            return new StackComponentPool<FloorController>(
                () => GamePrefabManager.GetGamePrefabStrictly<FloorPreset>(floorID).CreateController(),
                onReturnCallback: controller =>
                {
                    controller.SetActive(false);
                    controller.transform.SetParent(instance.cachedTransformContainer);
                });
        }

        public static FloorController Get(string floorID, Transform parent = null)
        {
            if (!poolsDictionary.ContainsKey(floorID))
            {
                poolsDictionary[floorID] = CreatePool(floorID);
            }

            return poolsDictionary[floorID].Get(parent);
        }

        public static void Return(string floorID, FloorController transform)
        {
            if (!poolsDictionary.ContainsKey(floorID))
            {
                poolsDictionary[floorID] = CreatePool(floorID);
            }

            poolsDictionary[floorID].Return(transform);
        }

        public static void Prewarm(string floorID, int count)
        {
            if (!poolsDictionary.ContainsKey(floorID))
            {
                poolsDictionary[floorID] = CreatePool(floorID);
            }

            poolsDictionary[floorID].Prewarm(count);
        }
    }
}
