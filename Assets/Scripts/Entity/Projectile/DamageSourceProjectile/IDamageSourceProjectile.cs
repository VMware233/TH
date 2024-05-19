using TH.Damage;

namespace TH.Entities
{
    public interface IDamageSourceProjectile
    {
        public DamagePacket damagePacket { get; }
    }

    public interface IDamageSourceProjectileInitInfo : IProjectileInitInfo
    {
        public DamagePacket damagePacket { get; }
    }
}
