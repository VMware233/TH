using Sirenix.OdinInspector;

namespace TH.Spells
{
    public partial class ProjectileUnitAction
    {
        public enum ProjectileSpawnPosition
        {
            [LabelText("施法者位置")]
            Caster,

            [LabelText("拥有者位置")]
            Owner
        }
    }
}