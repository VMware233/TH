using System;
using TH.Entities;
using VMFramework.Property;

namespace TH.Property
{
    public abstract class PropertyOfCreature : PropertyOfGameItem
    {
        protected Creature creature => (Creature)target;

        protected sealed override void OnTargetChanged(object previous, object current)
        {
            base.OnTargetChanged(previous, current);

            var previousCreature = (Creature)previous;
            var currentCreature = (Creature)current;

            OnCreatureChanged(previousCreature, currentCreature);
        }

        protected abstract void OnCreatureChanged(Creature previous, Creature current);
    }

    public abstract class CreaturePropertyConfig : PropertyConfig
    {
        public override Type targetType => typeof(Creature);

        protected override string idPrefix => "creature";

        public sealed override string GetValueString(object target)
        {
            return GetCreatureValueString((Creature)target);
        }

        protected abstract string GetCreatureValueString(Creature creature);
    }
}