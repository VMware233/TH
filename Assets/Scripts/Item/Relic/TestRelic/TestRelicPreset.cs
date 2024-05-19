using System;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using TH.Map;
using VMFramework.Configuration;
using VMFramework.Core;
using VMFramework.GameLogicArchitecture;
using VMFramework.OdinExtensions;

namespace TH.Items
{
    /// <summary>
    /// <see cref="TestRelic"/>的配置文件
    /// 具有<see cref="GamePrefabTypeAutoRegisterAttribute"/>，会自动创建并注册到<see cref="GamePrefabManager"/>中
    /// 在<see cref="TestRelic"/>中可以用下面代码获取配置文件
    /// <code>
    /// protected TestRelicPreset testRelicPreset => (TestRelicPreset)origin;
    /// </code>>
    /// </summary>
    [GamePrefabTypeAutoRegister(DEFAULT_ID)]
    public partial class TestRelicPreset : RelicPreset
    {
        public const string DEFAULT_ID = "test_relic";
        
        public override Type gameItemType => typeof(TestRelic);
        
        [LabelText("血量恢复"), TabGroup(TAB_GROUP_NAME, BASIC_SETTING_CATEGORY)]
        [Minimum(1)]
        [JsonProperty]
        public IChooserConfig<int> healthRestore = new SingleVectorChooserConfig<int>(1);

        [LabelText("最大血量恢复"), TabGroup(TAB_GROUP_NAME, BASIC_SETTING_CATEGORY)]
        // [MinValue("@healthRestore.GetValue()")]
        [JsonProperty]
        public int maxHealthRestore = 5;

        [LabelText("血量恢复间隔"), TabGroup(TAB_GROUP_NAME, BASIC_SETTING_CATEGORY)]
        [Unit(Units.Second)]
        [MinValue(0.1f)]
        [JsonProperty]
        public float healthRestoreInterval = 1;

        [LabelText("最大生命值加成"), TabGroup(TAB_GROUP_NAME, BASIC_SETTING_CATEGORY)]
        [MinValue(0)]
        public int maxHealthBonus = 10;

        public override void CheckSettings()
        {
            base.CheckSettings();
            
            // healthRestore.AssertIsAbove(0, nameof(healthRestore));
            // maxHealthRestore.AssertIsAboveOrEqual(healthRestore, nameof(maxHealthRestore),
            //     nameof(healthRestore));
            healthRestoreInterval.AssertIsAbove(0, nameof(healthRestoreInterval));
            
            maxHealthBonus.AssertIsAboveOrEqual(0, nameof(maxHealthBonus));
        }
    }
}