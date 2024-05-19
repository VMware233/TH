using System;
using TH.Map;
using VMFramework.Property;

namespace TH.Property
{
    public abstract class PropertyOfDamageableFloor : PropertyOfGameItem
    {
        protected DamageableFloor damageableFloor => (DamageableFloor)target;

        protected sealed override void OnTargetChanged(object previous, object current)
        {
            base.OnTargetChanged(previous, current);

            var previousDamageableFloor = (DamageableFloor)previous;
            var currentDamageableFloor = (DamageableFloor)current;

            OnDamageableFloorChanged(previousDamageableFloor, currentDamageableFloor);
        }

        protected abstract void OnDamageableFloorChanged(DamageableFloor previous, DamageableFloor current);
    }

    public abstract class DamageableFloorPropertyConfig : PropertyConfig
    {
        public override Type targetType => typeof(DamageableFloor);

        protected override string idPrefix => "damageable_floor";

        public sealed override string GetValueString(object target)
        {
            return GetDamageableFloorValueString((DamageableFloor)target);
        }

        protected abstract string GetDamageableFloorValueString(DamageableFloor damageableFloor);
    }
}