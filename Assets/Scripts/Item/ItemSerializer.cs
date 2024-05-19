using FishNet.Serializing;
using VMFramework.GameLogicArchitecture;

namespace TH.Items
{
    public static class ItemSerializer
    {
        public static void WriteItem(this Writer writer, Item item)
        {
            GameItem.WriteGameItem(writer, item);
        }

        public static Item ReadItem(this Reader reader)
        {
            return GameItem.ReadGameItem<Item>(reader);
        }
    }
}