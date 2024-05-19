#if UNITY_EDITOR
using VMFramework.Core;
using VMFramework.Editor;
using VMFramework.Editor.GameEditor;
using VMFramework.ExtendedTilemap;
using VMFramework.GameLogicArchitecture;

namespace TH.Map
{
    public partial class FloorPreset : IGameEditorMenuTreeNode
    {
        Icon IGameEditorMenuTreeNode.icon
        {
            get
            {
                if (tileID.IsNullOrEmpty())
                {
                    return Icon.None;
                }

                if (GamePrefabManager.TryGetGamePrefab(tileID, out ExtendedRuleTile ruleTile) == false)
                {
                    return Icon.None;
                }

                if (ruleTile is IGameEditorMenuTreeNode node)
                {
                    return node.icon;
                }
                
                return Icon.None;
            }
        }
    }
}
#endif