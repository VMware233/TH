namespace TH.Damage
{
    public interface IAttackOwner
    {
        /// <summary>
        /// 基础攻击力 = 基础攻击力基值 * 基础攻击力倍率
        /// </summary>
        public int attack { get; }

        /// <summary>
        /// 基础攻击力的基值
        /// </summary>
        public int attackBase { get; set; }

        /// <summary>
        /// 基础攻击力的倍率
        /// </summary>
        public float attackBoost { get; set; }

        /// <summary>
        /// 总攻击力的倍率 = 总攻击力倍率的基值 * 总攻击力倍率的倍率
        /// </summary>
        public float attackMultiplier { get; }

        /// <summary>
        /// 总攻击力倍率的基值
        /// </summary>
        public float attackMultiplierBase { get; set; }

        /// <summary>
        /// 总攻击力倍率的倍率
        /// </summary>
        public float attackMultiplierBoost { get; set; }

        /// <summary>
        /// 暴击率 = 暴击率的基值 * 暴击率的倍率
        /// </summary>
        public float criticalRate { get; }
        
        /// <summary>
        /// 暴击率的基值
        /// </summary>
        public float criticalRateBase { get; set; }

        /// <summary>
        /// 暴击率的倍率
        /// </summary>
        public float criticalRateBoost { get; set; }
        
        /// <summary>
        /// 暴击伤害倍率 = 暴击伤害倍率的基值 * 暴击伤害倍率的倍率
        /// </summary>
        public float criticalDamageMultiplier { get; }
        
        /// <summary>
        /// 暴击伤害倍率的基值
        /// </summary>
        public float criticalDamageMultiplierBase { get; set; }

        /// <summary>
        /// 暴击伤害倍率的倍率
        /// </summary>
        public float criticalDamageMultiplierBoost { get; set; }
    }
}