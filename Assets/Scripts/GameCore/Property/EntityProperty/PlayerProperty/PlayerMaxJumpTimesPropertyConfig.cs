using System;
using TH.Entities;
using VMFramework.GameLogicArchitecture;

namespace TH.Property
{
    public class MaxJumpTimesPropertyOfPlayer : PropertyOfPlayer
    {
        protected override void OnPlayerChanged(Player previous, Player current)
        {
            if (previous != null)
            {
                previous.maxJumpTimes.OnValueChanged -= OnIntValueChanged;
            }

            if (current != null)
            {
                current.maxJumpTimes.OnValueChanged += OnIntValueChanged;
            }
        }
    }

    [GamePrefabTypeAutoRegister(ID)]
    public class PlayerMaxJumpTimesPropertyConfig : PlayerPropertyConfig
    {
        public const string ID = "player_max_jump_times_property";

        public override Type gameItemType => typeof(MaxJumpTimesPropertyOfPlayer);

        protected override string GetPlayerValueString(Player player)
        {
            return $"{player.maxJumpTimes}";
        }
    }
}