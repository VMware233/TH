using FishNet.Serializing;
using UnityEngine.Scripting;
using VMFramework.GameLogicArchitecture;

namespace TH.Buffs
{
    [Preserve]
    public static class BuffSerializer
    {
        public static void WriteBuff(this Writer writer, IBuff buff)
        {
            buff.WriteGameItem(writer);
        }

        public static IBuff ReadBuff(this Reader reader)
        {
            return reader.ReadGameItem<IBuff>();
        }
    }
}