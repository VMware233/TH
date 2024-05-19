using System;
using TH.Entities;
using VMFramework.GameLogicArchitecture;

namespace TH.Property
{
    public class ExperienceBoostPropertyOfPlayer : PropertyOfPlayer
    {
        protected override void OnPlayerChanged(Player previous, Player current)
        {
            if (previous != null)
            {
                previous.experienceBoost.OnValueChanged -= OnBaseBoostFloatValueChanged;
            }

            if (current != null)
            {
                current.experienceBoost.OnValueChanged += OnBaseBoostFloatValueChanged;
            }
        }
    }

    [GamePrefabTypeAutoRegister(ID)]
    public class PlayerExperienceBoostPropertyConfig : PlayerPropertyConfig
    {
        public const string ID = "player_experience_boost_property";

        public override Type gameItemType => typeof(ExperienceBoostPropertyOfPlayer);

        protected override string GetPlayerValueString(Player player)
        {
            return $"{player.experienceBoost * 100:0.0}%";
        }
    }
}