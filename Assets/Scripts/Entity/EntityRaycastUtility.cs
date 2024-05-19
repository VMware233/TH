using UnityEngine;

namespace TH.Entities
{
    public static class EntityRaycastUtility
    {
        public static bool TryGetPlayer(this RaycastHit2D hit, out Player player)
        {
            var playerController = hit.transform.GetComponent<PlayerController>();
        
            if (playerController != null)
            {
                player = playerController.player;
                return true;
            }
        
            player = null;
            return false;
        }
    
        public static bool TryGetEntity(this RaycastHit2D hit, out Entity entity)
        {
            var entityController = hit.transform.GetComponent<EntityController>();
        
            if (entityController != null)
            {
                entity = entityController.entity;
                return true;
            }
        
            entity = null;
            return false;
        }
    }
}