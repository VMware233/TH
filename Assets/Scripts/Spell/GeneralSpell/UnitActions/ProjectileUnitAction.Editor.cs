#if UNITY_EDITOR
using TH.Entities;
using VMFramework.Configuration;

namespace TH.Spells
{
    public partial class ProjectileUnitAction
    {
        protected override void OnInspectorInit()
        {
            base.OnInspectorInit();

            linearProjectileID ??= new SingleGamePrefabIDChooserValue<ILinearProjectileConfig>();
            seekerProjectileID ??= new SingleGamePrefabIDChooserValue<ISeekerProjectileConfig>();
            laserProjectileID ??= new SingleGamePrefabIDChooserValue<IFixedLaserBeamConfig>();

            projectileScatterAngle ??= new SingleVectorChooserConfig<float>(30);

            projectileNumbers ??= new SingleVectorChooserConfig<int>(1);

            projectileInterval ??= new SingleVectorChooserConfig<float>(0);

            projectileSpeed ??= new SingleVectorChooserConfig<float>(10);

            delay ??= new SingleVectorChooserConfig<float>(0);

            physicalAttack ??= new SingleVectorChooserConfig<int>(1);
            magicalAttack ??= new SingleVectorChooserConfig<int>(0);
        }
    }
}
#endif