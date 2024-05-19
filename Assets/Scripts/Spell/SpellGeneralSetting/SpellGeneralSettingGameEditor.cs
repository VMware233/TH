#if UNITY_EDITOR
using VMFramework.Editor;
using VMFramework.Editor.GameEditor;
using VMFramework.GameLogicArchitecture;
using VMFramework.Localization;

namespace TH.Spells
{
    public partial class SpellGeneralSetting : IGameEditorMenuTreeNode
    {
        string INameOwner.name => new LocalizedTempString()
        {
            { "zh-CN", "法术" },
            { "en-US", "Spell" }
        };
    }
}
#endif