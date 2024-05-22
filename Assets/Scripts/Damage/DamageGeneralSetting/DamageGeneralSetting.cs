using Sirenix.OdinInspector;
using UnityEngine;
using VMFramework.GameLogicArchitecture;
using VMFramework.OdinExtensions;

namespace TH.Damage
{
    public sealed partial class DamageGeneralSetting : GeneralSetting
    {
        private const string DAMAGE_UI_CATEGORY = "伤害UI设置";

        private const string HEALTH_CHANGE_POPUP_UI_COLOR_CATEGORY =
            TAB_GROUP_NAME + "/" + DAMAGE_UI_CATEGORY + "/血量变化弹窗颜色";
        
        [LabelText("血量变化弹窗预设"), TabGroup(TAB_GROUP_NAME, DAMAGE_UI_CATEGORY)]
        [UIPresetID]
        [IsNotNullOrEmpty]
        public string healthChangePopupUIPanelID;
        
        [LabelText("血量减少UI颜色"), HorizontalGroup(HEALTH_CHANGE_POPUP_UI_COLOR_CATEGORY)]
        public Color healthReductionColor;
        
        [LabelText("血量恢复UI颜色"), HorizontalGroup(HEALTH_CHANGE_POPUP_UI_COLOR_CATEGORY)]
        public Color healthRegainColor;
    }
}
