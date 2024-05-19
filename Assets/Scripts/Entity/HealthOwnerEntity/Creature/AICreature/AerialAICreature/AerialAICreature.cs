using System;

namespace TH.Entities
{
    public class AerialAICreature : AICreature
    {

    }

    public class AerialAICreatureConfig : AICreatureConfig
    {
        public override Type gameItemType => typeof(AerialAICreature);

        protected override Type controllerType => typeof(AerialAICreatureController);
    }
}
