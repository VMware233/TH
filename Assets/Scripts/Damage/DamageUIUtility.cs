using TH.GameCore;
using UnityEngine;
using VMFramework.Core;
using VMFramework.UI;

namespace TH.Damage
{
    public static class DamageUIUtility
    {
        public static void PopupHealthChange(int healthChange, Vector3 position)
        {
            var value = healthChange.Abs();

            Color textColor = healthChange > 0
                ? GameSetting.damageGeneralSetting.healthRegainColor
                : GameSetting.damageGeneralSetting.healthReductionColor;
            
            PopupManager.PopupText(GameSetting.damageGeneralSetting.healthChangePopupUIPanelID,
                position, healthChange, textColor);
        }
    }
}