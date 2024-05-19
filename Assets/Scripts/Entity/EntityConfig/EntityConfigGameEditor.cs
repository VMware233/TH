#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VMFramework.Core;
using VMFramework.Editor;

namespace TH.Entities
{
    public partial class EntityConfig : IGameEditorMenuTreeNode
    {
        Icon IGameEditorMenuTreeNode.icon
        {
            get
            {
                if (prefab == null)
                {
                    return Icon.None;
                }

                if (prefab.TryGetComponent<EntityController>(out var entityController))
                {
                    var graphic = entityController.graphicTransform;

                    if (graphic != null)
                    {
                        if (graphic.TryGetComponent<SpriteRenderer>(out var spriteRenderer))
                        {
                            var sprite = spriteRenderer.sprite;

                            if (sprite != null)
                            {
                                return sprite;
                            }
                        }
                    }
                }
                
                return AssetPreview.GetAssetPreview(prefab).ToSprite();
            }
        }
    }
}
#endif