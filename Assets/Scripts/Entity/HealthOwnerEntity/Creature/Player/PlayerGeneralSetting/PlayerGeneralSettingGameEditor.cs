#if UNITY_EDITOR
using TH.GameCore;
using VMFramework.Editor;
using VMFramework.Editor.GameEditor;
using VMFramework.GameLogicArchitecture;
using VMFramework.Localization;

namespace TH.Entities
{
    public partial class PlayerGeneralSetting : IGameEditorMenuTreeNode
    {
        string INameOwner.name => new LocalizedTempString()
        {
            { "zh-CN", "玩家" },
            { "en-US", "Player" }
        };

        string IGameEditorMenuTreeNode.folderPath =>
            (GameSetting.entityGeneralSetting as IGameEditorMenuTreeNode)?.nodePath;
    }
}
#endif