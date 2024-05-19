using TH.Spells;
using VMFramework.GameLogicArchitecture;

namespace TH.Entities
{
    public interface IProjectile : IGameItem
    {
        public Entity sourceEntity { get; }
    }

    public interface IProjectileInitInfo
    {
        public Entity sourceEntity { get; }
        public Spell sourceSpell { get; }
    }

    public interface IProjectileConfig : ILocalizedGameTypedGamePrefab
    {

    }
}