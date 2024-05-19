using System;
using Sirenix.OdinInspector;
using VMFramework.GameLogicArchitecture;

namespace TH.Entities
{
    [GamePrefabTypeAutoRegister(ID)]
    public class CrouchStateConfig : PlayerActionStateConfig
    {
        public const string ID = "crouch_player_state";

        public override Type gameItemType => typeof(CrouchState);

        [LabelText("射线数量"), TabGroup(TAB_GROUP_NAME, BASIC_SETTING_CATEGORY)]
        [PropertyRange(2, 12)]
        public int raycastCount;
    }
}