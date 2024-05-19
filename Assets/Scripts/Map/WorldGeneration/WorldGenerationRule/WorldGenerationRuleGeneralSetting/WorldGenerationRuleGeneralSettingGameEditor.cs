#if UNITY_EDITOR
using TH.GameCore;
using VMFramework.Editor;
using VMFramework.GameLogicArchitecture;
using VMFramework.Localization;

namespace TH.Map
{
    public partial class WorldGenerationRuleGeneralSetting : IGameEditorMenuTreeNode
    {
        string INameOwner.name => new LocalizedTempString()
        {
            { "zh-CN", "世界生成规则" },
            { "en-US", "World Generation Rule" }
        };

        string IGameEditorMenuTreeNode.folderPath =>
            (GameSetting.worldGeneralSetting as IGameEditorMenuTreeNode)?.nodePath;
    }
}
#endif