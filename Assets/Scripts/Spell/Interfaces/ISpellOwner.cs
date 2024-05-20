using UnityEngine;

namespace TH.Spells
{
    public interface ISpellOwner
    {
        /// <summary>
        /// 施法的位置
        /// </summary>
        public Vector2 castPosition { get; }
    }
}
