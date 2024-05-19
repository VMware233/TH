using System.Collections.Generic;
using VMFramework.Configuration;
using VMFramework.GameLogicArchitecture;
using VMFramework.Core;
using Sirenix.OdinInspector;
using UnityEngine;
using VMFramework.OdinExtensions;

namespace TH.Map
{
    public partial class WorldGenerationRule : GamePrefab
    {
        #region Categories

        protected const string MAIN_PROCEDURE_CATEGORY = "主流程设置";

        protected const string BRANCH_PROCEDURE_CATEGORY = "分支流程设置";

        protected const string POST_PROCESSOR_CATEGORY = "后处理设置";

        #endregion

        protected override string idSuffix => "rule";

        [LabelText("第一个房间信息"), TabGroup(TAB_GROUP_NAME, MAIN_PROCEDURE_CATEGORY)]
        public IChooserConfig<RoomGenerationProcedureUnit> firstRoomInfo =
            new SingleValueChooserConfig<RoomGenerationProcedureUnit>();

        [LabelText("第一个房间的有效出口方向"), TabGroup(TAB_GROUP_NAME, MAIN_PROCEDURE_CATEGORY)]
        public FourTypesDirection2D firstRoomValidExitGatewayDirection;

        [LabelText("房间生成主流程"), TabGroup(TAB_GROUP_NAME, MAIN_PROCEDURE_CATEGORY)]
        [IsNotNullOrEmpty]
        public List<IChooserConfig<RoomGenerationProcedure>> mainRoomProcedures = new();

        [LabelText("房间生成分支流程"), TabGroup(TAB_GROUP_NAME, BRANCH_PROCEDURE_CATEGORY)]
        public List<IChooserConfig<RoomGenerationProcedure>> branchRoomProcedures = new();

        [LabelText("房间生成后处理"), TabGroup(TAB_GROUP_NAME, POST_PROCESSOR_CATEGORY)]
        public List<WorldGenerationPostProcessor> postProcessors = new();

        [LabelText("出入口格子最大失配率"), TabGroup(TAB_GROUP_NAME, MAIN_PROCEDURE_CATEGORY)]
        [PropertyRange(0f, 1f)]
        public float gatewayMismatchRate = 0.5f;

        [LabelText("分支只能生成在主支上"), TabGroup(TAB_GROUP_NAME, BRANCH_PROCEDURE_CATEGORY)]
        public bool branchOnlyOnMainBranch = true;

        [LabelText("分支生成的最小深度"), TabGroup(TAB_GROUP_NAME, BRANCH_PROCEDURE_CATEGORY)]
        public int branchMinDepth = 3;

        [LabelText("分支生成的据主支最大深度的距离"), TabGroup(TAB_GROUP_NAME, BRANCH_PROCEDURE_CATEGORY)]
        [ShowIf(nameof(branchOnlyOnMainBranch))]
        public int branchMaxMainBranchDepthDistance = 2;

        [LabelText("分支生成的据全局最大深度的距离"), TabGroup(TAB_GROUP_NAME, BRANCH_PROCEDURE_CATEGORY)]
        [HideIf(nameof(branchOnlyOnMainBranch))]
        public int branchMaxGlobalDepthDistance = 2;

        [LabelText("分支评价范围尺寸"), TabGroup(TAB_GROUP_NAME, BRANCH_PROCEDURE_CATEGORY)]
        public Vector2Int branchEvaluationRangeSize = new(7, 7);

        [LabelText("分支矩形内空缺权重"), TabGroup(TAB_GROUP_NAME, BRANCH_PROCEDURE_CATEGORY)]
        public int branchRectEmptyWeight = 1;

        [LabelText("距其他分支最小深度距离权重"), TabGroup(TAB_GROUP_NAME, BRANCH_PROCEDURE_CATEGORY)]
        public int branchMinDepthDistanceToOtherBranchWeight = 2;

        public override void CheckSettings()
        {
            base.CheckSettings();

            branchEvaluationRangeSize.AssertIsAllOdd(nameof(branchEvaluationRangeSize));
            
            firstRoomInfo.CheckSettings();

            mainRoomProcedures.CheckSettings();
            branchRoomProcedures.CheckSettings();

            postProcessors.CheckSettings();
        }

        protected override void OnInit()
        {
            base.OnInit();
            
            firstRoomInfo.Init();

            mainRoomProcedures.Init();
            branchRoomProcedures.Init();
            
            postProcessors.Init();
        }
    }
}
