#if UNITY_EDITOR
using VMFramework.Editor;
using VMFramework.Editor.GameEditor;
using VMFramework.GameLogicArchitecture;
using VMFramework.Localization;

namespace TH.Items
{
    public partial class ItemGeneralSetting : IGameEditorMenuTreeNode
    {
        string INameOwner.name => new LocalizedTempString()
        {
            { "zh-CN", "物品" },
            { "en-US", "Item" }
        };
    }
}
#endif