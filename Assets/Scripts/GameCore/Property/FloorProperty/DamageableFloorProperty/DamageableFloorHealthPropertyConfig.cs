using System;
using TH.Map;
using VMFramework.GameLogicArchitecture;

namespace TH.Property
{
    public class HealthPropertyOfDamageableFloor : PropertyOfDamageableFloor
    {
        protected override void OnDamageableFloorChanged(DamageableFloor previous,
            DamageableFloor current)
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
    public class DamageableFloorHealthPropertyConfig : DamageableFloorPropertyConfig
    {
        public const string ID = "damageable_floor_health_property";

        public override Type gameItemType => typeof(HealthPropertyOfDamageableFloor);

        protected override string GetDamageableFloorValueString(
            DamageableFloor damageableFloor)
        {
            return $"{damageableFloor.health}";
        }
    }
}