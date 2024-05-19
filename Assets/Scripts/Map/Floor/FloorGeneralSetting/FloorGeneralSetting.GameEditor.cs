#if UNITY_EDITOR
using TH.GameCore;
using VMFramework.Editor;
using VMFramework.GameLogicArchitecture;
using VMFramework.Localization;

namespace TH.Map
{
    public partial class FloorGeneralSetting : IGameEditorMenuTreeNode
    {
        string INameOwner.name => new LocalizedTempString()
        {
            { "zh-CN", "地板" },
            { "en-US", "Floor" }
        };

        string IGameEditorMenuTreeNode.folderPath =>
            (GameSetting.worldGeneralSetting as IGameEditorMenuTreeNode)?.nodePath;
    }
}
#endif