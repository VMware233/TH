using System;
using VMFramework.GameLogicArchitecture;

namespace TH.Entities
{
    [GamePrefabTypeAutoRegister(ID)]
    public class IdleStateConfig : PlayerActionStateConfig
    {
        public const string ID = "idle_player_state";
        
        public override Type gameItemType => typeof(IdleState);
    }
}