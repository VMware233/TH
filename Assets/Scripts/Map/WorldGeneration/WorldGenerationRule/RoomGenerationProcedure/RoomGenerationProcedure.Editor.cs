#if UNITY_EDITOR
using Sirenix.OdinInspector;
using VMFramework.Configuration;
using VMFramework.OdinExtensions;

namespace TH.Map
{
    public partial class RoomGenerationProcedure
    {
        protected override void OnInspectorInit()
        {
            base.OnInspectorInit();

            units ??= new();
        }

        private IChooserConfig<RoomGenerationProcedureUnit> AddUnitToListGUI()
        {
            return new SingleValueChooserConfig<RoomGenerationProcedureUnit>(new());
        }

        [Button]
        private void AddSeveralUnitsToList(int count,
            [GameTypeID] string roomTypeID)
        {
            for (int i = 0; i < count; i++)
            {
                units.Add(new SingleValueChooserConfig<RoomGenerationProcedureUnit>(new()
                {
                    roomGameTypeID = roomTypeID
                }));
            }
        }
    }
}
#endif