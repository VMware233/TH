using FishNet.Connection;
using FishNet.Object;
using TH.GameCore;
using TH.Items;
using UnityEngine;
using VMFramework.Containers;
using VMFramework.GameLogicArchitecture;
using VMFramework.Procedure;

namespace TH.Entities
{
    [ManagerCreationProvider(nameof(GameManagerType.Entity))]
    public class ItemDropManager : NetworkManagerBehaviour<ItemDropManager>
    {
        #region Add Item Drop

        public static void AddItemDrop(IContainer container, ItemDrop itemDrop)
        {
            if (container == null)
            {
                Debug.LogWarning($"{nameof(container)}为Null");
                return;
            }

            if (instance.IsServerStarted)
            {
                AddItemDropInstantaneously(container, itemDrop);
            }
            else
            {
                instance.AddItemDropRequest(container.uuid, itemDrop.uuid);
            }
        }

        private static void AddItemDropInstantaneously(IContainer container, ItemDrop itemDrop)
        {
            if (ContainerManager.TryGetInfo(container.uuid, out var containerInfo) ==
                false)
            {
                Debug.LogWarning($"不存在此{container.uuid}对应的Container");
                return;
            }
            
            var item = itemDrop.item;

            if (item == null)
            {
                Debug.LogWarning($"{itemDrop}没有{nameof(Item)}");
                return;
            }

            var itemClone = item.GetClone();

            if (container.TryAddItem(item) == false)
            {
                itemClone.count.value -= item.count.value;
            }
            else
            {
                itemDrop.SetItem(null);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void AddItemDropRequest(string containerUUID, string itemDropUUID,
            NetworkConnection connection = null)
        {
            if (ContainerManager.TryGetInfo(containerUUID, out var containerInfo) ==
                false)
            {
                Debug.LogWarning($"不存在此{containerUUID}对应的Container");
                return;
            }

            if (EntityManager.TryGetInfo(itemDropUUID, out var entityInfo) == false)
            {
                Debug.LogWarning(
                    $"不存在{itemDropUUID}对应的{nameof(EntityManager.EntityInfo)}");
                return;
            }

            if (entityInfo.owner is not ItemDrop itemDrop)
            {
                Debug.LogWarning(
                    $"{itemDropUUID}对应的{nameof(Entities)}不是{nameof(ItemDrop)}");
                return;
            }

            AddItemDropInstantaneously(containerInfo.owner, itemDrop);
        }

        #endregion
    }
}