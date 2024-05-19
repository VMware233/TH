using FishNet.Serializing;
using UnityEngine.Scripting;
using VMFramework.GameLogicArchitecture;

namespace TH.Map
{
    [Preserve]
    public static class FloorSerializer
    {
        public static void WriteFloor(this Writer writer, Floor floor)
        {
            GameItem.WriteGameItem(writer, floor);
        }

        public static Floor ReadFloor(this Reader reader)
        {
            return GameItem.ReadGameItem<Floor>(reader);
        }
    }
}