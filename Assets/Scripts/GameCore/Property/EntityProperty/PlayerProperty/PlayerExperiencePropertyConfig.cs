using System;
using TH.Entities;
using VMFramework.GameLogicArchitecture;

namespace TH.Property
{
    public class ExperiencePropertyOfPlayer : PropertyOfPlayer
    {
        protected override void OnPlayerChanged(Player previous, Player current)
        {
            if (previous != null)
            {
                previous.experience.OnValueChanged -= OnIntValueChanged;
            }

            if (current != null)
            {
                current.experience.OnValueChanged += OnIntValueChanged;
            }
        }
    }

    [GamePrefabTypeAutoRegister(ID)]
    public class PlayerExperiencePropertyConfig : PlayerPropertyConfig
    {
        public const string ID = "player_experience_property";

        public override Type gameItemType => typeof(ExperiencePropertyOfPlayer);

        protected override string GetPlayerValueString(Player player)
        {
            return $"{player.experience}";
        }
    }
}