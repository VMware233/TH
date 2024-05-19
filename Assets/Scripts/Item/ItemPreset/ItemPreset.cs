using System;
using VMFramework.GameLogicArchitecture;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TH.Items
{
    public partial class ItemPreset : DescribedGamePrefab
    {
        public override Type gameItemType => typeof(Item);

        protected override string idSuffix => "item";

        [LabelText("有限的最大堆叠"), TabGroup(TAB_GROUP_NAME, BASIC_SETTING_CATEGORY)]
        public bool finiteMaxStackCount = true;

        [LabelText("最大堆叠"), TabGroup(TAB_GROUP_NAME, BASIC_SETTING_CATEGORY)]
        [Indent]
        [EnableIf(nameof(finiteMaxStackCount))]
        public int maxStackCount = 64;

        [LabelText("图标"), TabGroup(TAB_GROUP_NAME, BASIC_SETTING_CATEGORY)]
        [Required, PreviewField(50, ObjectFieldAlignment.Center)]
        public Sprite icon;
    }
}
