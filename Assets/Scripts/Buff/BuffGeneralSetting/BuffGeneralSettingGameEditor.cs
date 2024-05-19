#if UNITY_EDITOR
using VMFramework.Editor;
using VMFramework.Editor.GameEditor;
using VMFramework.GameLogicArchitecture;

namespace TH.Buffs
{
    public partial class BuffGeneralSetting : IGameEditorMenuTreeNode
    {
        string INameOwner.name => "Buff";
    }
}
#endif