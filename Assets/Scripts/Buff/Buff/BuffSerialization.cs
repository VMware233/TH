using FishNet.Serializing;

namespace TH.Buffs
{
    public partial class Buff
    {
        protected override void OnWrite(Writer writer)
        {
            base.OnWrite(writer);

            writer.WriteSingle(duration, AutoPackType.PackedLess);
            writer.WriteInt32(level);
            writer.WriteString(uuid);
        }

        protected override void OnRead(Reader reader)
        {
            base.OnRead(reader);

            duration = new(reader.ReadSingle(AutoPackType.PackedLess));
            level = new(reader.ReadInt32());
            SetUUID(reader.ReadString());
        }
    }
}