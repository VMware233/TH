using VMFramework.Core;
using UnityEngine;

namespace TH.Entities
{
    public class ItemDropController : EntityController
    {
        public ItemDrop itemDrop => entity as ItemDrop;

        public void SetIcon(Sprite sprite)
        {
            var spriteRenderer = graphicTransform.GetOrAddComponent<SpriteRenderer>();

            spriteRenderer.sprite = sprite;
        }
    }
}
