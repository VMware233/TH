#if UNITY_EDITOR
using TH.GameCore;
using VMFramework.Editor;
using VMFramework.GameLogicArchitecture;
using VMFramework.Localization;

namespace TH.Map
{
    public partial class BiomeGeneralSetting : IGameEditorMenuTreeNode
    {
        string INameOwner.name => new LocalizedTempString()
        {
            { "zh-CN", "生物群系" },
            { "en-US", "Biome" }
        };

        string IGameEditorMenuTreeNode.folderPath =>
            (GameSetting.worldGeneralSetting as IGameEditorMenuTreeNode)?.nodePath;
    }
}
#endif