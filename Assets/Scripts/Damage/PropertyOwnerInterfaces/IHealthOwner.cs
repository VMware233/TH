namespace TH.Damage
{
    public interface IHealthOwner
    {
        /// <summary>
        /// 生命值
        /// </summary>
        public int health { get; set; }
        
        /// <summary>
        /// 最大生命值 = 最大生命值基值 * 最大生命值倍率
        /// </summary>
        public int maxHealth { get; }
        
        /// <summary>
        /// 最大生命值的基值
        /// </summary>
        public int maxHealthBase { get; set; }
        
        /// <summary>
        /// 最大生命值的倍率
        /// </summary>
        public float maxHealthBoost { get; set; }
    }
}