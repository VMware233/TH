using System;
using VMFramework.GameLogicArchitecture;

namespace TH.Map
{
    public sealed partial class BiomeGeneralSetting : GamePrefabGeneralSetting
    {
        #region Meta Data
        
        public override Type baseGamePrefabType => typeof(Biome);

        #endregion
    }
}
