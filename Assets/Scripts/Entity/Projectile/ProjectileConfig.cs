using System;
using Sirenix.OdinInspector;

namespace TH.Entities
{
    public abstract class ProjectileConfig : EntityConfig, IProjectileConfig
    {
        protected const string PROJECTILE_CATEGORY = "投掷物";

        protected override string idSuffix => "projectile";

        public override Type gameItemType => typeof(Projectile);

        protected override Type controllerType => typeof(ProjectileController);

        [LabelText("最大生命周期"), TabGroup(TAB_GROUP_NAME, PROJECTILE_CATEGORY)]
        [Unit(Units.Second)]
        public float maxLifeTime;
    }
}