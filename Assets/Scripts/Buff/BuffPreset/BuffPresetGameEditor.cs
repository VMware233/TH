#if UNITY_EDITOR
using VMFramework.Editor;
using VMFramework.Editor.GameEditor;

namespace TH.Buffs
{
    public partial class BuffPreset : IGameEditorMenuTreeNode
    {
        Icon IGameEditorMenuTreeNode.icon => icon;
    }
}
#endif