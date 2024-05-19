using TH.Damage;
using UnityEngine;

namespace TH.Spells
{
    public interface ISpellCaster
    {
        public string uuid { get; }

        public Vector2 casterPosition { get; }

        public Vector2 castPosition { get; }

        public void ProduceDamagePacket(IDamageable target, out DamagePacket packet);
    }
}
