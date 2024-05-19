using System;
using TH.Spells;
using VMFramework.Property;

namespace TH.Property
{
    public abstract class PropertyOfSpell : PropertyOfGameItem
    {
        protected sealed override void OnTargetChanged(object previous, object current)
        {
            base.OnTargetChanged(previous, current);

            var previousSpell = (Spell)previous;
            var currentSpell = (Spell)current;

            OnSpellChanged(previousSpell, currentSpell);
        }

        protected abstract void OnSpellChanged(Spell previous, Spell current);
    }

    public abstract class SpellPropertyConfig : PropertyConfig
    {
        public override Type targetType => typeof(Spell);

        protected override string idPrefix => "spell";

        public sealed override string GetValueString(object target)
        {
            return GetSpellValueString((Spell)target);
        }

        protected abstract string GetSpellValueString(Spell spell);
    }
}
