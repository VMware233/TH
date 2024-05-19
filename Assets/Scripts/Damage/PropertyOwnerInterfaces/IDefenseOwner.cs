namespace TH.Damage
{
    public interface IDefenseOwner
    {
        /// <summary>
        /// 防御力 = 防御力基值 * 防御力倍率
        /// </summary>
        public int defense { get; }
        
        /// <summary>
        /// 防御力的基值
        /// </summary>
        public int defenseBase { get; set; }
        
        /// <summary>
        /// 防御力的倍率
        /// </summary>
        public float defenseBoost { get; set; }
        
        /// <summary>
        /// 百分比防御力 = 百分比防御力的基值 * 百分比防御力的倍率
        /// </summary>
        public float defensePercent { get; }
        
        /// <summary>
        /// 百分比防御力的基值
        /// </summary>
        public float defensePercentBase { get; set; }
        
        /// <summary>
        /// 百分比防御力的倍率
        /// </summary>
        public float defensePercentBoost { get; set; }
    }
}
