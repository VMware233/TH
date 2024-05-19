#if UNITY_EDITOR
using VMFramework.Configuration;

namespace TH.Map
{
    public partial class WorldGenerationRule
    {
        protected override void OnInspectorInit()
        {
            base.OnInspectorInit();

            firstRoomInfo ??= new SingleValueChooserConfig<RoomGenerationProcedureUnit>();
            mainRoomProcedures ??= new();
            branchRoomProcedures ??= new();
            postProcessors ??= new();
        }
    }
}
#endif