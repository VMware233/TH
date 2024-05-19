#if UNITY_EDITOR
using VMFramework.Editor;

namespace TH.Buffs
{
    public partial class BuffPreset : IGameEditorMenuTreeNode
    {
        Icon IGameEditorMenuTreeNode.icon => icon;
    }
}
#endif