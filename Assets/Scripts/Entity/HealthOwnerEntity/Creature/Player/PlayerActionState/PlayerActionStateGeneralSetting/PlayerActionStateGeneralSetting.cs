using System;
using VMFramework.GameLogicArchitecture;

namespace TH.Entities
{
    public sealed partial class PlayerActionStateGeneralSetting : GamePrefabGeneralSetting
    {
        #region Meta Data

        public override Type baseGamePrefabType => typeof(PlayerActionStateConfig);

        public override string gameItemName => nameof(PlayerActionState);

        #endregion
    }
}
