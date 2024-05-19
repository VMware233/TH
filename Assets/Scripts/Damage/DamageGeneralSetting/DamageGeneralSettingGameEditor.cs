#if UNITY_EDITOR
using VMFramework.Editor;
using VMFramework.GameLogicArchitecture;
using VMFramework.Localization;

namespace TH.Damage
{
    public partial class DamageGeneralSetting : IGameEditorMenuTreeNode
    {
        string INameOwner.name => new LocalizedTempString()
        {
            { "zh-CN", "伤害" },
            { "en-US", "Damage" }
        };
    }
}
#endif