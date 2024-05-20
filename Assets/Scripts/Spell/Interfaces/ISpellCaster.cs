using TH.Damage;
using UnityEngine;
using VMFramework.Network;

namespace TH.Spells
{
    public interface ISpellCaster : IUUIDOwner
    {
        /// <summary>
        /// 施法者的位置
        /// </summary>
        public Vector2 casterPosition { get; }

        /// <summary>
        /// 施法的位置
        /// </summary>
        public Vector2 castPosition { get; }

        /// <summary>
        /// 产生伤害包
        /// </summary>
        /// <param name="target"></param>
        /// <param name="packet"></param>
        public void ProduceDamagePacket(IDamageable target, out DamagePacket packet);
    }
}
