using Sirenix.OdinInspector;
using VMFramework.Configuration;

namespace TH.Spells
{
    public abstract partial class SpellUnitAction : BaseConfig
    {
        [LabelText("支持的目标类型")]
        [PropertyOrder(-1000)]
        [ShowInInspector]
        public abstract SpellTargetType supportedTargetType { get; }

        public abstract void Examine(Spell spell, Spell.SpellCastInfo spellCastInfo,
            GeneralSpell.SpellOperationToken operationToken);
    }
}