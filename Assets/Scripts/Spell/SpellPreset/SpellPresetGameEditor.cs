#if UNITY_EDITOR
using VMFramework.Editor;
using VMFramework.Editor.GameEditor;

namespace TH.Spells
{
    public partial class SpellPreset : IGameEditorMenuTreeNode
    {
        Icon IGameEditorMenuTreeNode.icon => icon;
    }
}
#endif