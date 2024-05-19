using System;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using VMFramework.Core;

namespace TH.Items
{
    public class YinYangOrbRelicPreset : RelicPreset
    {
        public override Type gameItemType => typeof(YinYangOrbRelic);

        [LabelText("¹¥»÷Á¦"), TabGroup(TAB_GROUP_NAME, BASIC_SETTING_CATEGORY)]
        [MinValue(1)]
        [JsonProperty]
        public int attackBasePower = 1;


        public override void CheckSettings()
        {
            base.CheckSettings();

            attackBasePower.AssertIsAbove(0, nameof(attackBasePower));
        }
    }
}