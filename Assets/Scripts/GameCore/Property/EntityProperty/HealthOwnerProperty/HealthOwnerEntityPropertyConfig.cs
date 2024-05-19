using System;
using TH.Entities;
using VMFramework.Property;

namespace TH.Property
{
    public abstract class PropertyOfHealthOwnerEntity : PropertyOfGameItem
    {
        protected HealthOwnerEntity healthOwnerEntity => (HealthOwnerEntity)target;

        protected sealed override void OnTargetChanged(object previous, object current)
        {
            base.OnTargetChanged(previous, current);

            var previousHealthOwnerEntity = (HealthOwnerEntity)previous;
            var currentHealthOwnerEntity = (HealthOwnerEntity)current;

            OnHealthOwnerEntityChanged(previousHealthOwnerEntity, currentHealthOwnerEntity);
        }

        protected abstract void OnHealthOwnerEntityChanged(HealthOwnerEntity previous,
            HealthOwnerEntity current);
    }

    public abstract class HealthOwnerEntityPropertyConfig : PropertyConfig
    {
        public override Type targetType => typeof(HealthOwnerEntity);

        protected override string idPrefix => "health_owner_entity";

        public sealed override string GetValueString(object target)
        {
            return GetHealthOwnerEntityValueString((HealthOwnerEntity)target);
        }

        protected abstract string GetHealthOwnerEntityValueString(HealthOwnerEntity healthOwnerEntity);
    }
}