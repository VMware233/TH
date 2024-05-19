using System;

namespace TH.Entities
{
    public class GeneralTerrestrialAICreatureConfig : AICreatureConfig
    {
        public override Type gameItemType => typeof(GeneralTerrestrialAICreature);

        protected override Type controllerType => typeof(GeneralTerrestrialAICreatureController);
    }
}