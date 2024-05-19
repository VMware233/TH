using System;

namespace TH.Items
{
    public class RelicPreset : ItemPreset
    {
        public override Type gameItemType => typeof(Relic);

        protected override string idSuffix => "relic";
    }
}
