using System;
using TH.Entities;
using VMFramework.GameLogicArchitecture;

namespace TH.Property
{
    public class MovementSpeedPropertyOfCreature : PropertyOfCreature
    {
        protected override void OnCreatureChanged(Creature previous, Creature current)
        {
            if (previous != null)
            {
                previous.movementSpeed.OnValueChanged -= OnBaseBoostFloatValueChanged;
            }

            if (current != null)
            {
                current.movementSpeed.OnValueChanged += OnBaseBoostFloatValueChanged;
            }
        }

        public override string GetTooltipTitle()
        {
            return $"{creature.movementSpeed.baseValue:0} * " +
                   $"{creature.movementSpeed.boostValue * 100:0.0}%";
        }
    }

    [GamePrefabTypeAutoRegister(ID)]
    public class CreatureMovementSpeedProperty : CreaturePropertyConfig
    {
        public const string ID = "creature_movement_speed_property";

        public override Type gameItemType => typeof(MovementSpeedPropertyOfCreature);

        protected override string GetCreatureValueString(Creature creature)
        {
            return $"{creature.movementSpeed:0}";
        }
    }
}