#if UNITY_EDITOR
using VMFramework.Editor;
using VMFramework.GameLogicArchitecture;
using VMFramework.Localization;

namespace TH.Entities
{
    public partial class EntityGeneralSetting : IGameEditorMenuTreeNode
    {
        string INameOwner.name => new LocalizedTempString()
        {
            { "zh-CN", "实体" },
            { "en-US", "Entity" }
        };
    }
}
#endif