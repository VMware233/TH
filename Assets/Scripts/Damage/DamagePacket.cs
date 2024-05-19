namespace TH.Damage
{
    public struct DamagePacket
    {
        /// <summary>
        /// 伤害的直接来源
        /// </summary>
        public IDamageSource directSource;

        /// <summary>
        /// 是否是反弹攻击
        /// </summary>
        public bool isReflectedAttack;

        /// <summary>
        /// 是否是近战攻击
        /// </summary>
        public bool isMelee;

        /// <summary>
        /// 物理伤害
        /// </summary>
        public int physicalDamage;

        /// <summary>
        /// 魔法伤害
        /// </summary>
        public int magicalDamage;
        
        /// <summary>
        /// 总伤害倍率
        /// </summary>
        public float damageMultiplier;

        /// <summary>
        /// 暴击率
        /// </summary>
        public float criticalRate;

        /// <summary>
        /// 暴击伤害倍率
        /// </summary>
        public float criticalDamageMultiplier;

        public DamagePacket(IDamageSource directSource)
        {
            this.directSource = directSource;
            this.isReflectedAttack = false;
            this.isMelee = false;
            this.physicalDamage = 0;
            this.magicalDamage = 0;
            this.damageMultiplier = 1;
            this.criticalRate = 0;
            this.criticalDamageMultiplier = 1;
        }
    }
}