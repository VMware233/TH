using VMFramework.Configuration;

namespace TH.Map
{
    public abstract class WorldGenerationPostProcessor : BaseConfig
    {
        public abstract void Process(WorldGenerator.WorldGenerationInfo info);
    }
}
