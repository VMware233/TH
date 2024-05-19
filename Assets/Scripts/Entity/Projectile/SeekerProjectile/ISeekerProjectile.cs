using UnityEngine;

namespace TH.Entities
{
    public interface ISeekerProjectile : IProjectile
    {
        public Transform trackingTarget { get; }

        public Vector2 initialVelocity { get; }

        public void Init(ISeekerProjectileInitInfo initInfo);
    }

    public interface ISeekerProjectileInitInfo : IProjectileInitInfo
    {
        public Transform trackingTarget { get; }

        public Vector2 initialVelocity { get; }
    }

    public interface ISeekerProjectileConfig : IProjectileConfig
    {

    }
}
