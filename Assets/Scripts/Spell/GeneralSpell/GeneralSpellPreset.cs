using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using VMFramework.Configuration;
using VMFramework.OdinExtensions;

namespace TH.Spells
{
    public sealed partial class GeneralSpellPreset : SpellPreset
    {
        public override Type gameItemType => typeof(GeneralSpell);

        [LabelText("技能单元"), TabGroup(TAB_GROUP_NAME, SPELL_CATEGORY)]
        [IsNotNullOrEmpty]
        public List<SpellUnitAction> spellUnitActions = new();

        public override void CheckSettings()
        {
            base.CheckSettings();

            spellUnitActions.CheckSettings();
        }

        protected override void OnInit()
        {
            base.OnInit();

            spellUnitActions.Init();
        }
    }
}
