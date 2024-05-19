using System;
using TH.Spells;
using VMFramework.Core;
using VMFramework.GameLogicArchitecture;

namespace TH.Property
{
    public class CooldownPropertyOfSpell : PropertyOfSpell
    {
        protected override void OnSpellChanged(Spell previous, Spell current)
        {
            if (previous != null)
            {
                previous.cooldown.OnValueChanged -= OnFloatValueChanged;
            }

            if (current != null)
            {
                current.cooldown.OnValueChanged += OnFloatValueChanged;
            }
        }
    }

    [GamePrefabTypeAutoRegister(ID)]
    public class SpellCooldownPropertyConfig : SpellPropertyConfig
    {
        public const string ID = "spell_cooldown_property";

        public override Type gameItemType => typeof(CooldownPropertyOfSpell);

        protected override string GetSpellValueString(Spell spell)
        {
            return $"{spell.cooldown.value.ClampMin(0):0.0}";
        }
    }
}