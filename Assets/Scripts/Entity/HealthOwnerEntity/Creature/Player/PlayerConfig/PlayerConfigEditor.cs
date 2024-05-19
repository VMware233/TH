#if UNITY_EDITOR
namespace TH.Entities
{
    public partial class PlayerConfig
    {
        protected override void OnInspectorInit()
        {
            base.OnInspectorInit();

            initialPlayerActionStateConfigs ??= new();

            initialRelics ??= new();
        }
    }
}
#endif