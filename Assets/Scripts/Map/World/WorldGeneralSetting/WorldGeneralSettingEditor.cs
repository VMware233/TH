#if UNITY_EDITOR
namespace TH.Map
{
    public partial class WorldGeneralSetting
    {
        protected override void OnInspectorInit()
        {
            base.OnInspectorInit();

            container ??= new();
            container.SetDefaultContainerID("%Map");
        }
    }
}
#endif