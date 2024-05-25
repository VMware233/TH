#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.OdinInspector;
using UnityEngine;

namespace VMFramework.UI
{
    public partial class TracingUIManager
    {
        [Button]
        private static void StartTracing(UIPanelController uiPanelController, Transform target)
        {
            if (uiPanelController is ITracingUIPanel tracingUIPanel)
            {
                StartTracing(tracingUIPanel, new TracingConfig(target));
            }
        }

        [Button]
        private static void StopTracing(UIPanelController uiPanelController)
        {
            if (uiPanelController is ITracingUIPanel tracingUIPanel)
            {
                StopTracing(tracingUIPanel);
            }
        }
    }
}
#endif