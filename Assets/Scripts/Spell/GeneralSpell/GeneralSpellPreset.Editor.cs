#if UNITY_EDITOR
namespace TH.Spells
{
    public partial class GeneralSpellPreset
    {
        protected override void OnInspectorInit()
        {
            base.OnInspectorInit();

            spellUnitActions ??= new();
        }
    }
}
#endif