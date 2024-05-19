using Sirenix.OdinInspector;
using UnityEngine;
using VMFramework.GameLogicArchitecture;

namespace TH.Buffs
{
    public partial class BuffPreset : DescribedGamePrefab
    {
        protected const string BUFF_CATEGORY = "Buff";

        protected override string idSuffix => "buff";

        [LabelText("图标"), TabGroup(TAB_GROUP_NAME, BUFF_CATEGORY)]
        [Required]
        [PreviewField(50, ObjectFieldAlignment.Center)]
        public Sprite icon;

        [LabelText("最大层数"), TabGroup(TAB_GROUP_NAME, BUFF_CATEGORY)]
        [MinValue(1)]
        public int maxLevel = 1;

        [LabelText("层数是否可叠加"), TabGroup(TAB_GROUP_NAME, BUFF_CATEGORY)]
        public bool isLevelStackable = true;

        [LabelText("是否是独立Buff"), SuffixLabel("若新Buff层数不同且持续时间高则新开一个定时器"),
         TabGroup(TAB_GROUP_NAME, BUFF_CATEGORY)]
        [HideIf(nameof(isLevelStackable))]
        public bool isIndependentBuff = false;

        [LabelText("层数叠加种类"), TabGroup(TAB_GROUP_NAME, BUFF_CATEGORY)]
        [ShowIf(nameof(isLevelStackable))]
        public LevelStackType levelStackType;

        [LabelText("持续时间叠加种类"), TabGroup(TAB_GROUP_NAME, BUFF_CATEGORY)]
        public DurationStackType durationStackType;
    }
}