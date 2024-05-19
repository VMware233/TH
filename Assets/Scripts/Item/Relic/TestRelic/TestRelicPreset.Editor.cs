#if UNITY_EDITOR
using VMFramework.Configuration;

namespace TH.Items
{
    public partial class TestRelicPreset
    {
        protected override void OnInspectorInit()
        {
            base.OnInspectorInit();

            healthRestore ??= new SingleVectorChooserConfig<int>(1);
        }
    }
}
#endif