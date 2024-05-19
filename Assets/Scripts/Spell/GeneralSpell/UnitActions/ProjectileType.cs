using Sirenix.OdinInspector;

namespace TH.Spells
{
    public partial class ProjectileUnitAction
    {
        public enum ProjectileType
        {
            [LabelText("线性")]
            LinearProjectile,

            [LabelText("追踪")]
            SeekerProjectile,

            [LabelText("激光")]
            FixedLaserBeam,
        }
    }
}