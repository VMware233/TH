using System;

namespace TH.Damage
{
    public interface IDamageable
    {
        public event Action<DamageResult> OnDamageTaken; 
        
        /// <summary>
        /// 产生伤害结果
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="result"></param>
        public void ProduceDamageResult(DamagePacket packet, out DamageResult result)
        {
            this.DefaultProcessDamagePacket(packet, out result);
        }

        /// <summary>
        /// 处理伤害结果
        /// </summary>
        /// <param name="result"></param>
        public void ProcessDamageResult(DamageResult result)
        {
            
        }

        /// <summary>
        /// 是否可以受到伤害
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public bool CanBeDamaged(IDamageSource source)
        {
            return true;
        }
    }
}
