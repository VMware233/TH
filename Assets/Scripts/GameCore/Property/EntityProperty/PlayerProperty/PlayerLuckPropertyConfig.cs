using System;
using TH.Entities;
using VMFramework.GameLogicArchitecture;

namespace TH.Property
{
    public class LuckPropertyOfPlayer : PropertyOfPlayer
    {
        protected override void OnPlayerChanged(Player previous, Player current)
        {
            if (previous != null)
            {
                previous.luck.OnValueChanged -= OnIntValueChanged;
            }

            if (current != null)
            {
                current.luck.OnValueChanged += OnIntValueChanged;
            }
        }
    }

    [GamePrefabTypeAutoRegister(ID)]
    public class PlayerLuckPropertyConfig : PlayerPropertyConfig
    {
        public const string ID = "player_luck_property";

        public override Type gameItemType => typeof(LuckPropertyOfPlayer);

        protected override string GetPlayerValueString(Player player)
        {
            return $"{player.luck}";
        }
    }
}