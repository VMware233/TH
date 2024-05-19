using VMFramework.Configuration;

namespace TH.Items
{
    public class ItemGenerationConfig : DefaultContainerItemGenerationConfig<Item, ItemPreset>
    {
        public override Item GenerateItem()
        {
            int currentCount = count.GetValue();

            if (currentCount == 0)
            {
                return null;
            }

            return Item.Create(itemID, currentCount);
        }
    }
}