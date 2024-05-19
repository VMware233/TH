using System;
using Sirenix.OdinInspector;
using UnityEngine;
using VMFramework.GameLogicArchitecture;
using VMFramework.OdinExtensions;

namespace TH.Entities
{
    public sealed partial class EntityGeneralSetting : GamePrefabGeneralSetting
    {
        #region Meta Data

        public override Type baseGamePrefabType => typeof(EntityConfig);

        public override string gameItemName => nameof(Entity);

        #endregion

        [field: LabelText("实体Layer")]
        [field: Layer]
        [field: SerializeField]
        public int entityLayer { get; private set; }
    }
}
