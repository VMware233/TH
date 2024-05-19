using System;
using TH.Entities;
using VMFramework.GameLogicArchitecture;

namespace TH.Property
{
    public class HealthPropertyOfHealthOwnerEntity : PropertyOfHealthOwnerEntity
    {
        protected override void OnHealthOwnerEntityChanged(HealthOwnerEntity previous,
            HealthOwnerEntity current)
        {
            if (previous != null)
            {
                previous.health.OnValueChanged -= OnIntValueChanged;
            }

            if (current != null)
            {
                current.health.OnValueChanged += OnIntValueChanged;
            }
        }
    }

    [GamePrefabTypeAutoRegister(ID)]
    public class HealthOwnerEntityHealthPropertyConfig : HealthOwnerEntityPropertyConfig
    {
        public const string ID = "health_owner_entity_health_property";

        public override Type gameItemType => typeof(HealthPropertyOfHealthOwnerEntity);

        protected override string GetHealthOwnerEntityValueString(
            HealthOwnerEntity healthOwnerEntity)
        {
            return $"{healthOwnerEntity.health} / {healthOwnerEntity.maxHealth}";
        }
    }
}