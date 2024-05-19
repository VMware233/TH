using FishNet.Serializing;
using VMFramework.GameLogicArchitecture;

namespace TH.Buffs
{
    public static class BuffSerializer
    {
        public static void WriteBuff(this Writer writer, Buff buff)
        {
            GameItem.WriteGameItem(writer, buff);
        }

        public static Buff ReadBuff(this Reader reader)
        {
            return GameItem.ReadGameItem<Buff>(reader);
        }
    }
}