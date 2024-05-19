using System;
using VMFramework.GameLogicArchitecture;
using VMFramework.Core;
using UnityEngine;
using Sirenix.OdinInspector;
using VMFramework.OdinExtensions;

namespace TH.Map
{
    public sealed partial class FloorGeneralSetting : GamePrefabGeneralSetting
    {
        #region Meta Data
        
        public override Type baseGamePrefabType => typeof(FloorPreset);

        public override string gameItemName => nameof(Floor);

        #endregion

        [field: LabelText("地板Layer")]
        [field: Layer]
        [field: SerializeField]
        public int floorLayer { get; private set; }

        [field: LabelText("地板Cell Size")]
        [field: SerializeField]
        public Vector2 cellSize { get; private set; } = new Vector2(1, 1);

        public override void CheckSettings()
        {
            base.CheckSettings();

            floorLayer.AssertIsAboveOrEqual(0, nameof(floorLayer));
        }
    }
}