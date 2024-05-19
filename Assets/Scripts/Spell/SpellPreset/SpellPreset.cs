using System;
using VMFramework.GameLogicArchitecture;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TH.Spells
{
    public partial class SpellPreset : DescribedGamePrefab
    {
        protected const string SPELL_CATEGORY = "法术";

        protected override string idSuffix => "spell";

        public override Type gameItemType => typeof(Spell);

        [LabelText("图标"), TabGroup(TAB_GROUP_NAME, SPELL_CATEGORY)]
        [PreviewField(50, ObjectFieldAlignment.Center)]
        [Required]
        public Sprite icon;

        [LabelText("冷却时间"), SuffixLabel("秒"), TabGroup(TAB_GROUP_NAME, SPELL_CATEGORY)]
        [MinValue(0)]
        public float maxCooldown;
    }
}
