using System;
using VMFramework.GameLogicArchitecture;

namespace TH.Entities
{
    public class PlayerActionStateConfig : GameTypedGamePrefab
    {
        protected override string idSuffix => "player_state";

        public override Type gameItemType => typeof(PlayerActionState);
    }
}