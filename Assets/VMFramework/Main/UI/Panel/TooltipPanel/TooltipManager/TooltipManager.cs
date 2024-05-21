using UnityEngine;
using VMFramework.Procedure;

namespace VMFramework.UI
{
    [ManagerCreationProvider(ManagerType.UICore)]
    public class TooltipManager : UniqueMonoBehaviour<TooltipManager>
    {
        public static void Open(ITooltipProvider tooltipProvider, IUIPanelController source)
        {
            if (tooltipProvider == null)
            {
                Debug.LogWarning($"{nameof(tooltipProvider)} is Null");
                return;
            }

            if (tooltipProvider.ShowTooltip() == false)
            {
                return;
            }

            var tooltipID = tooltipProvider.GetTooltipID();

            var tooltip = UIPanelPool.GetUniquePanelStrictly<ITooltip>(tooltipID);

            tooltip.Open(tooltipProvider, source);
        }

        public static void Close(ITooltipProvider tooltipProvider)
        {
            if (tooltipProvider == null)
            {
                Debug.LogWarning($"{nameof(tooltipProvider)} is Null");
                return;
            }

            var tooltipID = tooltipProvider.GetTooltipID();

            var tooltip = UIPanelPool.GetUniquePanelStrictly<ITooltip>(tooltipID);

            tooltip.Close(tooltipProvider);
        }
    }
}