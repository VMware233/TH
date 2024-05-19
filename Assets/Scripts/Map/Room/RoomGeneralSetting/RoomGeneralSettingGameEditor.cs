#if UNITY_EDITOR
using TH.GameCore;
using VMFramework.Editor;
using VMFramework.GameLogicArchitecture;
using VMFramework.Localization;

namespace TH.Map
{
    public partial class RoomGeneralSetting : IGameEditorMenuTreeNode, IGameEditorMenuTreeNodesProvider
    {
        string INameOwner.name => new LocalizedTempString()
        {
            { "zh-CN", "房间" },
            { "en-US", "Room" }
        };

        string IGameEditorMenuTreeNode.folderPath =>
            (GameSetting.worldGeneralSetting as IGameEditorMenuTreeNode)?.nodePath;

        bool IGameEditorMenuTreeNodesProvider.isMenuTreeNodesVisible => false;
    }
}
#endif