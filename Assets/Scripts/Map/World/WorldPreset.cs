using System;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using VMFramework.GameLogicArchitecture;
using VMFramework.OdinExtensions;

namespace TH.Map
{
    public class WorldPreset : LocalizedGameTypedGamePrefab
    {
        public override Type gameItemType => typeof(World);

        [LabelText("生成规则")]
        [GamePrefabID(typeof(WorldGenerationRule))]
        [IsNotNullOrEmpty]
        [JsonProperty]
        public string generationRuleID;
    }
}