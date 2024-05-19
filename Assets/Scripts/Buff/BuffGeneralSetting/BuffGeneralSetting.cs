using System;
using VMFramework.GameLogicArchitecture;

namespace TH.Buffs
{
    public sealed partial class BuffGeneralSetting : GamePrefabGeneralSetting
    {
        #region Meta Data

        public override Type baseGamePrefabType => typeof(BuffPreset);

        public override string gameItemName => nameof(Buff);

        #endregion
    }
}