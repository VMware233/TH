using System;
using VMFramework.GameLogicArchitecture;

namespace TH.Entities
{
    [GamePrefabTypeAutoRegister(ID)]
    public class JumpStateConfig : PlayerActionStateConfig
    {
        public const string ID = "jump_player_state";

        public override Type gameItemType => typeof(JumpState);
    }
}