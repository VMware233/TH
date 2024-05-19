using System;
using VMFramework.GameLogicArchitecture;

namespace TH.Items
{
    public sealed partial class ItemGeneralSetting : GamePrefabGeneralSetting
    {
        #region Meta Data

        public override Type baseGamePrefabType => typeof(ItemPreset);

        public override string gameItemName => nameof(Item);

        #endregion
    }
}
