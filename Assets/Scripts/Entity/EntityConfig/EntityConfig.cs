using System;
using VMFramework.GameLogicArchitecture;
using Sirenix.OdinInspector;
using UnityEngine;
using VMFramework.OdinExtensions;

namespace TH.Entities
{
    public partial class EntityConfig : DescribedGamePrefab
    {
        protected virtual Type controllerType => typeof(EntityController);

        public override Type gameItemType => typeof(Entity);

        protected override string idSuffix => "entity";

        [LabelText("预热数量"), TabGroup(TAB_GROUP_NAME, BASIC_SETTING_CATEGORY)]
        [MinValue(0), MaxValue(1000)]
        public int prewarmCount = 0;

        [LabelText("预制体"), TabGroup(TAB_GROUP_NAME, BASIC_SETTING_CATEGORY)]
        [Required, RequiredComponent(nameof(controllerType)), AssetsOnly]
        public GameObject prefab;
    }
}
