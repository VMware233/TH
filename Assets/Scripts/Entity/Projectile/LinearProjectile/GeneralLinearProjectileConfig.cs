using System;

namespace TH.Entities
{
    public sealed class GeneralLinearProjectileConfig : DamageSourceProjectileConfig, ILinearProjectileConfig
    {
        public override Type gameItemType => typeof(GeneralLinearProjectile);

        protected override Type controllerType => typeof(GeneralLinearProjectileController);
    }
}