#if UNITY_EDITOR
using TH.GameCore;
using VMFramework.Editor;
using VMFramework.GameLogicArchitecture;
using VMFramework.Localization;

namespace TH.Entities
{
    public partial class ProjectileGeneralSetting : IGameEditorMenuTreeNode
    {
        string INameOwner.name => new LocalizedTempString()
        {
            { "zh-CN", "投射物" },
            { "en-US", "Projectile" }
        };

        string IGameEditorMenuTreeNode.folderPath =>
            (GameSetting.entityGeneralSetting as IGameEditorMenuTreeNode)?.nodePath;
    }
}
#endif