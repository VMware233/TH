using FishNet.Serializing;
using VMFramework.GameLogicArchitecture;

namespace TH.Entities
{
    public static class EntitySerializer
    {
        public static void WriteEntity(this Writer writer, Entity entity)
        {
            GameItem.WriteGameItem(writer, entity);
        }

        public static Entity ReadEntity(this Reader reader)
        {
            return GameItem.ReadGameItem<Entity>(reader);
        }
    }
}