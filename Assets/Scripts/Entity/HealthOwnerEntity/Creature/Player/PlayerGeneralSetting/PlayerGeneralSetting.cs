using VMFramework.Core;
using Sirenix.OdinInspector;
using UnityEngine;
using VMFramework.GameLogicArchitecture;
using VMFramework.OdinExtensions;

namespace TH.Entities
{
    public sealed partial class PlayerGeneralSetting : GeneralSettingBase
    {
        [LabelText("每级的经验")]
        public int experiencePerLevel = 100;

        [field: LabelText("玩家Layer")]
        [field: Layer]
        [field: SerializeField]
        public int playerLayer { get; private set; }
        
        [field: LabelText("默认玩家ID")]
        [field: GamePrefabID(typeof(PlayerConfig))]
        [field: SerializeField]
        public string defaultPlayerID { get; private set; }
        
        public override void CheckSettings()
        {
            base.CheckSettings();

            playerLayer.AssertIsAboveOrEqual(0, nameof(playerLayer));
        }
    }
}
