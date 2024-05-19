using VMFramework.Configuration;

namespace TH.Items
{
    public class RelicGenerationConfig : DefaultContainerItemGenerationConfig<Relic, RelicPreset>
    {
        public override Relic GenerateItem()
        {
            int currentCount = count.GetValue();

            if (currentCount == 0)
            {
                return null;
            }

            return Item.Create<Relic>(itemID, currentCount);
        }
    }
}