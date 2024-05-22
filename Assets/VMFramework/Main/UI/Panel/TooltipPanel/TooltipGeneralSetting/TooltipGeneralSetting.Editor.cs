#if UNITY_EDITOR
namespace VMFramework.UI
{
    public partial class TooltipGeneralSetting
    {
        protected override void OnInspectorInit()
        {
            base.OnInspectorInit();

            tooltipBindConfigs ??= new();
            
            tooltipPriorityPresets ??= new();
        }
    }
}
#endif