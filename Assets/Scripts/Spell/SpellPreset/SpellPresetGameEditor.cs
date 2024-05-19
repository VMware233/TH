#if UNITY_EDITOR
using VMFramework.Editor;

namespace TH.Spells
{
    public partial class SpellPreset : IGameEditorMenuTreeNode
    {
        Icon IGameEditorMenuTreeNode.icon => icon;
    }
}
#endif