using FishNet.Serializing;
using UnityEngine.Scripting;
using VMFramework.GameLogicArchitecture;

namespace TH.Map
{
    [Preserve]
    public static class WorldSerializer
    {
        public static void WriteWorld(this Writer writer, World world)
        {
            GameItem.WriteGameItem(writer, world);
        }

        public static World ReadWorld(this Reader reader)
        {
            return GameItem.ReadGameItem<World>(reader);
        }
    }
}