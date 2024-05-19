using System;
using VMFramework.GameLogicArchitecture;

namespace TH.Map
{
    public sealed partial class WorldGenerationRuleGeneralSetting : GamePrefabGeneralSetting
    {
        #region Meta Data
        
        public override Type baseGamePrefabType => typeof(WorldGenerationRule);

        #endregion
    }
}
