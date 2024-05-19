using UnityEngine;

namespace TH.Entities
{
    public interface IFixedLaserBeam : IProjectile
    {
        public Vector2 direction { get; }

        public void Init(IFixedLaserBeamInitInfo initInfo);
    }

    public interface IFixedLaserBeamInitInfo : IProjectileInitInfo
    {
        public Vector2 direction { get; }
    }

    public interface IFixedLaserBeamConfig : IProjectileConfig
    {

    }
}