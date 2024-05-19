#if UNITY_EDITOR
using VMFramework.Editor;

namespace TH.Items
{
    public partial class ItemPreset : IGameEditorMenuTreeNode
    {
        public EditorIconType iconType => EditorIconType.Sprite;

        Icon IGameEditorMenuTreeNode.icon => icon;
    }
}
#endif