#if UNITY_EDITOR
using VMFramework.Editor;
using VMFramework.Editor.GameEditor;

namespace TH.Items
{
    public partial class ItemPreset : IGameEditorMenuTreeNode
    {
        public EditorIconType iconType => EditorIconType.Sprite;

        Icon IGameEditorMenuTreeNode.icon => icon;
    }
}
#endif