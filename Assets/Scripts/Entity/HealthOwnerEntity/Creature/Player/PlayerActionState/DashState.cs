using VMFramework.Core;
using Sirenix.OdinInspector;
using TH.GameEvents;
using UnityEngine;
using VMFramework.Core.FSM;
using VMFramework.GameEvents;

namespace TH.Entities
{
    public class DashState : PlayerActionState, IMultiFSMState<string, PlayerController>
    {
        protected DashStateConfig dashStateConfig => (DashStateConfig)gamePrefab;

        [ShowInInspector]
        private int dashTimes = 0;

        [ShowInInspector]
        private float dashTimer = 0;

        [ShowInInspector]
        private float lastGravityScale = 0;

        public override void OnInit()
        {
            base.OnInit();

            GameEventManager.AddCallback(PlayerGameEvents.DASH, Dash);
        }

        private void Dash(BoolInputGameEvent boolEvent)
        {
            ExitThisState(boolEvent);
            EnterThisState(boolEvent);
        }

        void IMultiFSMState<string, PlayerController>.OnEnter()
        {
            if (dashTimes >= dashStateConfig.maxDashTimes)
            {
                return;
            }

            dashTimes++;

            dashTimer = 0;

            lastGravityScale = fsm.owner.rigidbody.gravityScale;

            fsm.owner.rigidbody.gravityScale = 0;

            var direction = GameEventManager.GetVector2(PlayerGameEvents.DIRECTION);

            if (fsm.owner.isGrounded)
            {
                if (direction.y.Abs() < 0.1f)
                {
                    direction.y += dashStateConfig.YIncreaseOnGround;
                }
            }

            direction = direction.normalized;

            var velocity = direction * dashStateConfig.dashForce;

            if (velocity.y > 0)
            {
                velocity.y *= dashStateConfig.positiveYScale;
            }
            else if (velocity.y < 0)
            {
                velocity.y *= dashStateConfig.negativeYScale;
            }

            fsm.owner.rigidbody.velocity = velocity;
        }

        void IMultiFSMState<string, PlayerController>.OnExit()
        {
            fsm.owner.rigidbody.gravityScale = lastGravityScale;
        }

        void IMultiFSMState<string, PlayerController>.Update(bool isActive)
        {
            if (isActive)
            {
                if (dashTimer >= dashStateConfig.dashTime)
                {
                    fsm.ExitState(dashStateConfig.id);
                }
                else
                {
                    dashTimer += Time.deltaTime;
                }
            }

            if (fsm.owner.fallGrounded)
            {
                if (fsm.HasCurrentState(dashStateConfig.id))
                {
                    fsm.ExitState(dashStateConfig.id);
                }

                dashTimes = 0;
            }
        }
    }
}
