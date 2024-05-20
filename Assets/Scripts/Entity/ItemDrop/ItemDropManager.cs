using FishNet.Connection;
using FishNet.Object;
using TH.GameCore;
using TH.Items;
using UnityEngine;
using VMFramework.Containers;
using VMFramework.GameLogicArchitecture;
using VMFramework.Network;
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
            if (UUIDCoreManager.CheckConsistency(container) == false)
            {
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
            if (UUIDCoreManager.TryGetOwnerWithWarning(containerUUID, out IContainer container) ==
                false)
            {
                return;
            }

            if (UUIDCoreManager.TryGetOwnerWithWarning(itemDropUUID, out Entity entity) == false)
            {
                return;
            }

            if (entity is not ItemDrop itemDrop)
            {
                Debug.LogWarning($"{itemDropUUID}对应的{nameof(Entities)}不是{nameof(ItemDrop)}");
                return;
            }

            AddItemDropInstantaneously(container, itemDrop);
        }

        #endregion
    }
}