using System;
using TH.Entities;
using VMFramework.GameLogicArchitecture;

namespace TH.Property
{
    public class FlySpeedPropertyOfPlayer : PropertyOfPlayer
    {
        protected override void OnPlayerChanged(Player previous, Player current)
        {
            if (previous != null)
            {
                previous.flySpeed.OnValueChanged -= OnBaseBoostFloatValueChanged;
            }

            if (current != null)
            {
                current.flySpeed.OnValueChanged += OnBaseBoostFloatValueChanged;
            }
        }

        public override string GetTooltipTitle()
        {
            return $"{player.flySpeed.baseValue:0} * " +
                   $"{player.flySpeed.boostValue * 100:0.0}%";
        }
    }

    [GamePrefabTypeAutoRegister(ID)]
    public class PlayerFlySpeedPropertyConfig : PlayerPropertyConfig
    {
        public const string ID = "player_fly_speed_property";

        public override Type gameItemType => typeof(FlySpeedPropertyOfPlayer);

        protected override string GetPlayerValueString(Player player)
        {
            return $"{player.flySpeed:0}";
        }
    }
}