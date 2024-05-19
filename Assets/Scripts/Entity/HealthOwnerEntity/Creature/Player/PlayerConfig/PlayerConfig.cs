using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TH.Items;
using TH.Spells;
using VMFramework.Configuration;
using VMFramework.GameLogicArchitecture;
using VMFramework.Containers;
using VMFramework.OdinExtensions;

namespace TH.Entities
{
    [GamePrefabTypeAutoRegister(ID)]
    public partial class PlayerConfig : CreatureConfig
    {
        protected const string PLAYER_CATEGORY = "玩家设置";

        public const string ID = "player_entity";

        public override Type gameItemType => typeof(Player);

        protected override Type controllerType => typeof(PlayerController);

        [LabelText("遗物容器"), TabGroup(TAB_GROUP_NAME, PLAYER_CATEGORY)]
        [GamePrefabID(typeof(ContainerPreset))]
        [IsNotNullOrEmpty]
        public string relicInventoryID;

        [LabelText("默认最大跳跃次数"), TabGroup(TAB_GROUP_NAME, PLAYER_CATEGORY)]
        [MinValue(1)]
        public int defaultMaxJumpTimes = 2;

        [LabelText("默认跳跃力"), TabGroup(TAB_GROUP_NAME, PLAYER_CATEGORY)]
        [MinValue(0)]
        public float defaultJumpForce = 10;

        [LabelText("默认飞行速度"), TabGroup(TAB_GROUP_NAME, PLAYER_CATEGORY)]
        [MinValue(0)]
        public float defaultFlySpeed = 10;

        [LabelText("默认幸运值"), TabGroup(TAB_GROUP_NAME, PLAYER_CATEGORY)]
        [MinValue(0)]
        public int defaultLuck = 0;

        [LabelText("默认金币数量"), TabGroup(TAB_GROUP_NAME, PLAYER_CATEGORY)]
        public int defaultCoinCount = 999;

        [TabGroup(TAB_GROUP_NAME, PLAYER_CATEGORY)]
        [GamePrefabID(typeof(SpellPreset))]
        public string spellOneID;

        [TabGroup(TAB_GROUP_NAME, PLAYER_CATEGORY)]
        [GamePrefabID(typeof(SpellPreset))]
        public string spellTwoID;

        [TabGroup(TAB_GROUP_NAME, PLAYER_CATEGORY)]
        [GamePrefabID(typeof(SpellPreset))]
        public string spellThreeID;

        [TabGroup(TAB_GROUP_NAME, PLAYER_CATEGORY)]
        [GamePrefabID(typeof(SpellPreset))]
        public string spellFourID;

        [LabelText("初始玩家动作状态配置"), TabGroup(TAB_GROUP_NAME, PLAYER_CATEGORY)]
        [ListDrawerSettings(ShowFoldout = false)]
        public List<InitialPlayerActionStateConfig> initialPlayerActionStateConfigs = new();

        [LabelText("初始遗物"), TabGroup(TAB_GROUP_NAME, PLAYER_CATEGORY)]
        [ListDrawerSettings(ShowFoldout = false)]
        public List<RelicGenerationConfig> initialRelics = new();

        public override void CheckSettings()
        {
            base.CheckSettings();
            
            initialPlayerActionStateConfigs.CheckSettings();
            initialRelics.CheckSettings();
        }

        protected override void OnInit()
        {
            base.OnInit();
            
            initialPlayerActionStateConfigs.Init();
            initialRelics.Init();
        }
    }

    public class InitialPlayerActionStateConfig : BaseConfig, IInitialPlayerActionStateConfig
    {
        [HideLabel, HorizontalGroup]
        [GamePrefabID(typeof(PlayerActionStateConfig))]
        public string playerActionStateID;

        [LabelText("是否自动进入"), LabelWidth(100), HorizontalGroup]
        public bool autoEnter;

        #region Interface

        string IInitialPlayerActionStateConfig.playerActionStateID => playerActionStateID;

        bool IInitialPlayerActionStateConfig.autoEnter => autoEnter;

        #endregion
    }

    public interface IInitialPlayerActionStateConfig
    {
        public string playerActionStateID { get; }
        public bool autoEnter { get; }
    }
}