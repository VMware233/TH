using TH.GameEvents;
using UnityEngine;
using VMFramework.Core.FSM;
using VMFramework.GameEvents;

namespace TH.Entities
{
    public class FlyState : PlayerActionState, IMultiFSMState<string, PlayerController>
    {
        protected FlyStateConfig flyStateConfig => (FlyStateConfig)gamePrefab;

        public override void OnInit()
        {
            GameEventManager.AddCallback(PlayerGameEvents.FLY, EnterThisState);
            GameEventManager.AddCallback(PlayerGameEvents.FLY_CANCEL, ExitThisState);
        }

        void IMultiFSMState<string, PlayerController>.OnEnter()
        {
            fsm.owner.rigidbody.gravityScale = 0;

            var direction = GameEventManager.GetVector2(PlayerGameEvents.DIRECTION);

            var targetVelocity = direction * player.flySpeed;

            fsm.owner.rigidbody.velocity = targetVelocity;
        }

        void IMultiFSMState<string, PlayerController>.OnExit()
        {
            fsm.owner.rigidbody.gravityScale = 4.5f;
        }

        void IMultiFSMState<string, PlayerController>.Update(bool isActive)
        {
            if (isActive)
            {
                var direction = GameEventManager.GetVector2(PlayerGameEvents.DIRECTION);

                // 根据输入方向和速度计算目标速度
                Vector2 targetVelocity = direction * player.flySpeed;

                // 平滑插值当前速度和目标速度
                fsm.owner.rigidbody.velocity = Vector2.Lerp(fsm.owner.rigidbody.velocity, targetVelocity,
                    Time.deltaTime * flyStateConfig.flySmoothness);
            }
        }
    }
}
