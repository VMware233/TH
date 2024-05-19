namespace TH.Damage
{
    public interface IDamageSource
    {
        /// <summary>
        /// 产生伤害包
        /// </summary>
        /// <param name="target"></param>
        /// <param name="packet"></param>
        public void ProduceDamagePacket(IDamageable target, out DamagePacket packet)
        {
            packet = default;
        }

        /// <summary>
        /// 是否可以对目标造成伤害
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool CanDamage(IDamageable target)
        {
            return true;
        }
    }
}
