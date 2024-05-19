using VMFramework.Core;
using Sirenix.OdinInspector;
using VMFramework.Core.FSM;
using Spine;
using TH.GameEvents;
using VMFramework.GameEvents;

namespace TH.Entities
{
    public class JumpState : PlayerActionState, IMultiFSMState<string, PlayerController>
    {
        protected JumpStateConfig jumpStateConfig => (JumpStateConfig)gamePrefab;

        [ShowInInspector]
        private int jumpTimes = 0;

        public override void OnInit()
        {
            base.OnInit();

            GameEventManager.AddCallback(PlayerGameEvents.JUMP, Jump);
        }

        private void Jump(BoolInputGameEvent boolEvent)
        {
            if (jumpTimes >= player.maxJumpTimes)
            {
                return;
            }

            jumpTimes++;

            fsm.owner.rigidbody.velocity = fsm.owner.rigidbody.velocity.ReplaceY(player.jumpForce);

            UpdateJumpAnimation();
        }

        private void UpdateJumpAnimation()
        {
            
            if (jumpTimes == 1)
            {
                TrackEntry animationEntry;
                animationEntry = fsm.owner.skeletonAnimation.AnimationState.SetAnimation(0, "jump", false);
                animationEntry.TimeScale = 2f; 
            }
            else
            {
                TrackEntry animationEntry;
                animationEntry = fsm.owner.skeletonAnimation.AnimationState.SetAnimation(0, "multi jump", false);
                animationEntry.TimeScale = 3f; 
            }
        }

        void IMultiFSMState<string, PlayerController>.Update(bool isActive)
        {
            if (fsm.owner.fallGrounded)
            {
                jumpTimes = 0;
            }
        }
    }
}
