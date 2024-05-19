using System;

namespace TH.Entities
{
    public class AICreatureConfig : CreatureConfig
    {
        public override Type gameItemType => typeof(AICreature);

        protected override Type controllerType => typeof(AICreatureController);
    }
}