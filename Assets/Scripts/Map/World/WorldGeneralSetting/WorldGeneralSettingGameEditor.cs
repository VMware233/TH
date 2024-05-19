#if UNITY_EDITOR
using VMFramework.Editor;
using VMFramework.GameLogicArchitecture;
using VMFramework.Localization;

namespace TH.Map
{
    public partial class WorldGeneralSetting : IGameEditorMenuTreeNode
    {
        string INameOwner.name => new LocalizedTempString()
        {
            { "zh-CN", "世界" },
            { "en-US", "World" }
        };
    }
}
#endif