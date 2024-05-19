#if UNITY_EDITOR
using TH.GameCore;
using VMFramework.Editor;
using VMFramework.GameLogicArchitecture;
using VMFramework.Localization;

namespace TH.Entities
{
    public partial class PlayerActionStateGeneralSetting : IGameEditorMenuTreeNode
    {
        string INameOwner.name => new LocalizedTempString()
        {
            { "zh-CN", "玩家动作状态" },
            { "en-US", "Player Action State" }
        };

        string IGameEditorMenuTreeNode.folderPath =>
            (GameSetting.entityGeneralSetting as IGameEditorMenuTreeNode)?.nodePath;
    }
}
#endif