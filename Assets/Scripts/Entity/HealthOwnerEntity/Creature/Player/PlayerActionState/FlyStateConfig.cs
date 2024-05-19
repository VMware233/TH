using System;
using VMFramework.GameLogicArchitecture;

namespace TH.Entities
{
    [GamePrefabTypeAutoRegister(ID)]
    public class FlyStateConfig : PlayerActionStateConfig
    {
        public const string ID = "fly_player_state";

        public override Type gameItemType => typeof(FlyState);

        public float flySmoothness = 10f;
    }
}