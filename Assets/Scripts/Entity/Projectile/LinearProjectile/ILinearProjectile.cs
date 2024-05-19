using UnityEngine;

namespace TH.Entities
{
    public interface ILinearProjectile : IProjectile
    {
        public Vector2 initialVelocity { get; }

        public void Init(ILinearProjectileInitInfo info);
    }

    public interface ILinearProjectileInitInfo : IProjectileInitInfo
    {
        public Vector2 initialVelocity { get; }
    }

    public interface ILinearProjectileConfig : IProjectileConfig
    {

    }
}