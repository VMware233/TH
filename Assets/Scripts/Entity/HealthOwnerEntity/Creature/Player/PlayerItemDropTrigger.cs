using FishNet;
using Sirenix.OdinInspector;
using TH.Items;
using UnityEngine;

namespace TH.Entities
{
    public class PlayerItemDropTrigger : MonoBehaviour
    {
        [Required]
        [SerializeField]
        private PlayerController playerController;

        private void Start()
        {
            enabled = InstanceFinder.IsClientStarted;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (playerController.IsOwner == false)
            {
                return;
            }

            var itemDropController = other.GetComponent<ItemDropController>();

            if (itemDropController == null)
            {
                return;
            }

            var itemDrop = itemDropController.itemDrop;

            if (itemDrop?.item is Relic)
            {
                ItemDropManager.AddItemDrop(playerController.player.relicInventory, itemDrop);
            }
        }
    }
}