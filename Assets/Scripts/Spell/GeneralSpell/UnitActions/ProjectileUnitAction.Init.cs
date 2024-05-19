namespace TH.Spells
{
    public partial class ProjectileUnitAction
    {
        public override void CheckSettings()
        {
            base.CheckSettings();
            
            linearProjectileID.CheckSettings();
            seekerProjectileID.CheckSettings();
            laserProjectileID.CheckSettings();
            
            projectileScatterAngle.CheckSettings();
            projectileNumbers.CheckSettings();
            projectileInterval.CheckSettings();
            projectileSpeed.CheckSettings();
            delay.CheckSettings();
            
            physicalAttack.CheckSettings();
            magicalAttack.CheckSettings();
        }

        protected override void OnInit()
        {
            base.OnInit();
            
            linearProjectileID.Init();
            seekerProjectileID.Init();
            laserProjectileID.Init();
            
            projectileScatterAngle.Init();
            projectileNumbers.Init();
            projectileInterval.Init();
            projectileSpeed.Init();
            delay.Init();
            
            physicalAttack.Init();
            magicalAttack.Init();
        }
    }
}