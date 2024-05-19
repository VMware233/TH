using System;
using TH.Entities;
using VMFramework.Property;

namespace TH.Property
{
    public abstract class PropertyOfEntity : PropertyOfGameItem
    {
        protected Entity entity => (Entity)target;

        protected sealed override void OnTargetChanged(object previous, object current)
        {
            base.OnTargetChanged(previous, current);

            var previousEntity = (Entity)previous;
            var currentEntity = (Entity)current;

            OnEntityChanged(previousEntity, currentEntity);
        }

        protected abstract void OnEntityChanged(Entity previous, Entity current);
    }

    public abstract class EntityPropertyConfig : PropertyConfig
    {
        public override Type targetType => typeof(Entity);

        protected override string idPrefix => "entity";

        public sealed override string GetValueString(object target)
        {
            return GetEntityValueString((Entity)target);
        }

        protected abstract string GetEntityValueString(Entity entity);
    }
}