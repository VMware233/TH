using System;
using VMFramework.GameLogicArchitecture;

namespace TH.Entities
{
    [GamePrefabTypeAutoRegister(ID)]
    public class DashStateConfig : PlayerActionStateConfig
    {
        public const string ID = "dash_player_state";

        public override Type gameItemType => typeof(DashState);

        public float dashForce = 5;

        public float dashTime = 0.5f;

        public float positiveYScale = 0.5f;

        public float negativeYScale = 0.5f;

        public int maxDashTimes = 2;

        public float YIncreaseOnGround = 0.3f;
    }
}