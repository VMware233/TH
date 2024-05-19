using System;
using VMFramework.Configuration;
using VMFramework.GameLogicArchitecture;
using Sirenix.OdinInspector;
using UnityEngine;
using VMFramework.OdinExtensions;

namespace TH.Map
{
    public sealed partial class WorldGeneralSetting : GamePrefabGeneralSetting
    {
        #region MetaData
        
        public override Type baseGamePrefabType => typeof(WorldPreset);

        public override string gameItemName => nameof(World);

        #endregion

        [LabelText("预制体"), TabGroup(TAB_GROUP_NAME, MISCELLANEOUS_CATEGORY)]
        [RequiredComponent(typeof(GameMap))]
        [RequiredComponent(typeof(GameMapNetwork))]
        public GameObject prefab;

        [HideLabel, TabGroup(TAB_GROUP_NAME, MISCELLANEOUS_CATEGORY)]
        public ContainerChooser container = new();

        [LabelText("默认世界"), TabGroup(TAB_GROUP_NAME, MISCELLANEOUS_CATEGORY)]
        [SuffixLabel("玩家一开始默认进入的")]
        [GamePrefabID(typeof(WorldPreset))]
        [IsNotNullOrEmpty]
        public string defaultWorldPreset;

        [LabelText("渲染半径"), TabGroup(TAB_GROUP_NAME, MISCELLANEOUS_CATEGORY)]
        public int renderRadius = 2;
    }
}