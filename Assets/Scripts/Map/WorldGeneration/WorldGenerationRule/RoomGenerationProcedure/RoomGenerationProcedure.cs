using System.Collections.Generic;
using Sirenix.OdinInspector;
using VMFramework.Configuration;
using VMFramework.Core;

namespace TH.Map
{
    public partial class RoomGenerationProcedure : BaseConfig
    {
        protected const string NEXT_ROOM_CHOOSE_WEIGHT = "下一房间选择权重";

        protected const string EXIT_GATEWAY_CHOOSE_WEIGHT = "出口选择权重";

        [LabelText("优先方向")]
        public FourTypesDirection2D priorityDirection;

        [LabelText("是优先方向权重"), BoxGroup(EXIT_GATEWAY_CHOOSE_WEIGHT)]
        public int isPriorityDirectionExitGatewayChooseWeight = 0;

        [LabelText("是连续方向权重"), BoxGroup(EXIT_GATEWAY_CHOOSE_WEIGHT)]
        public int isConsecutiveDirectionExitGatewayChooseWeight = 0;

        [LabelText("包含优先方向权重"), BoxGroup(NEXT_ROOM_CHOOSE_WEIGHT)]
        public int includingPriorityDirectionNextRoomChooseWeight = 5;

        [LabelText("包含连续方向权重"), BoxGroup(NEXT_ROOM_CHOOSE_WEIGHT)]
        public int includingConsecutiveDirectionNextRoomChooseWeight = 0;

        [LabelText("不包含连续方向权重"), BoxGroup(NEXT_ROOM_CHOOSE_WEIGHT)]
        public int excludingConsecutiveDirectionNextRoomChooseWeight = 0;

        [LabelText("有效出口数量权重"), BoxGroup(NEXT_ROOM_CHOOSE_WEIGHT)]
        public int validExitGatewayCountNextRoomChooseWeight = 1;

        [LabelText("有效单元数量权重"), BoxGroup(NEXT_ROOM_CHOOSE_WEIGHT)]
        public int validUnitCountNextRoomChooseWeight = 0;

        [LabelText("单元")]
#if UNITY_EDITOR
        [ListDrawerSettings(CustomAddFunction = nameof(AddUnitToListGUI))]
#endif
        public List<IChooserConfig<RoomGenerationProcedureUnit>> units = new();

        public override void CheckSettings()
        {
            base.CheckSettings();
            
            units.CheckSettings();
        }

        protected override void OnInit()
        {
            base.OnInit();
            
            units.Init();
        }
    }
}