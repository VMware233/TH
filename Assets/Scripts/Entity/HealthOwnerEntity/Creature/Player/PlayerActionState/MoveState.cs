using UnityEngine;
using Spine;
using TH.GameEvents;
using VMFramework.Core.FSM;
using VMFramework.GameEvents;

namespace TH.Entities
{
    public class MoveState : PlayerActionState, IMultiFSMState<string, PlayerController>
    {
        protected MoveStateConfig moveStateConfig => (MoveStateConfig)gamePrefab;

        public override void OnInit()
        {
            base.OnInit();
            //Calculate are run acceleration & deceleration forces using formula: amount = ((1 / Time.fixedDeltaTime) * acceleration) / runMaxSpeed
            moveStateConfig.runAccelAmount = (50 * moveStateConfig.runAcceleration) / player.movementSpeed;
            moveStateConfig.runDeccelAmount = (50 * moveStateConfig.runDecceleration) / player.movementSpeed;

            #region Variable Ranges

            moveStateConfig.runAcceleration =
                Mathf.Clamp(moveStateConfig.runAcceleration, 0.01f, player.movementSpeed);
            moveStateConfig.runDecceleration =
                Mathf.Clamp(moveStateConfig.runDecceleration, 0.01f, player.movementSpeed);

            #endregion
        }

        public void Update(bool isActive)
        {
            if (isActive)
            {
                //Debug.Log("fsm.owner.rigidbody.gravityScale:"+fsm.owner.rigidbody.gravityScale);

                //origin.LastOnGroundTime -= Time.deltaTime;//记录角色最后一次接触地面的时间。用于实现一些机制，例如在离开地面后仍然允许一定时间内进行跳跃（所谓的“地板延迟”）。
            }
        }

        public void FixedUpdate(bool isActive)
        {
            if (isActive)
            {
                Run(1);
            }
        }

        private void Run(float lerpAmount)
        {
            //根据玩家输入的移动值（左右键输入），计算水平方向上的目标速度
            float targetSpeed = GameEventManager.GetFloat(PlayerGameEvents.MOVE) * player.movementSpeed;

            //我们可以使用 Lerp() 减少弧度控制，这可以平滑弧度方向和速度的变化
            targetSpeed = Mathf.Lerp(fsm.owner.rigidbody.velocity.x, targetSpeed, lerpAmount);

            if (targetSpeed != 0f)
            {
                if (fsm.owner.fallGrounded)
                {
                    TrackEntry currentAnimation = fsm.owner.skeletonAnimation.AnimationState.GetCurrent(0);
                    if (currentAnimation == null || !currentAnimation.Animation.Name.Equals("run"))
                    {
                        fsm.owner.skeletonAnimation.AnimationState.SetAnimation(0, "run", true);
                    }
                }


                if (targetSpeed < 0.01f)
                {
                    fsm.owner.transform.localScale = new Vector3(1, 1, 1);
                }
                else
                {
                    fsm.owner.transform.localScale = new Vector3(-1, 1, 1);
                }
            }
            else
            {
                if (fsm.owner.fallGrounded)
                {
                    TrackEntry currentAnimation = fsm.owner.skeletonAnimation.AnimationState.GetCurrent(0);
                    if (currentAnimation == null || !currentAnimation.Animation.Name.Equals("idle"))
                    {
                        fsm.owner.skeletonAnimation.AnimationState.SetAnimation(0, "idle", true);
                    }
                }
                
            }


            #region Calculate AccelRate 计算加速度

            float accelRate;

            //根据我们是否加速（包括转弯）获取加速度值
            //或尝试减速（停止）。 如果我们在空中飞行，也会应用乘数。
            if (moveStateConfig.LastOnGroundTime > 0)
                accelRate = (Mathf.Abs(targetSpeed) > 0.01f)
                    ? moveStateConfig.runAccelAmount
                    : moveStateConfig.runDeccelAmount;
            else
                accelRate = (Mathf.Abs(targetSpeed) > 0.01f)
                    ? moveStateConfig.runAccelAmount * moveStateConfig.accelInAir
                    : moveStateConfig.runDeccelAmount * moveStateConfig.deccelInAir;

            #endregion

            //#region Add Bonus Jump Apex Acceleration 添加奖励跳跃顶点加速
            ////在跳跃的顶点时增加加速度和最大速度，使跳跃感觉更有弹性、反应灵敏和自然
            //if ((fsm.owner.playerParameter.isJumping || fsm.owner.playerParameter.isWallJumping || fsm.owner.playerParameter.isJumpFalling) &&
            //    Mathf.Abs(fsm.owner.rigidbody.velocity.y) <
            //    PlayerActionStateConfig.GetPrefab<JumpStateConfig>(JumpStateConfig.registeredID).jumpHangTimeThreshold)
            //{
            //    accelRate *= PlayerActionStateConfig.GetPrefab<JumpStateConfig>(JumpStateConfig.registeredID).jumpHangAccelerationMult;
            //    targetSpeed *= PlayerActionStateConfig.GetPrefab<JumpStateConfig>(JumpStateConfig.registeredID).jumpHangMaxSpeedMult;
            //}
            //#endregion

            #region Conserve Momentum 保存动力

            //如果玩家朝所需方向移动但速度高于其 maxSpeed，我们不会减慢他们的速度
            if (moveStateConfig.doConserveMomentum &&
                Mathf.Abs(fsm.owner.rigidbody.velocity.x) > Mathf.Abs(targetSpeed) &&
                Mathf.Sign(fsm.owner.rigidbody.velocity.x) == Mathf.Sign(targetSpeed) &&
                Mathf.Abs(targetSpeed) > 0.01f && moveStateConfig.LastOnGroundTime < 0)
            {
                //防止发生任何减速，或者换句话说，保存当前动量
                //您可以尝试允许玩家在这种“状态”下稍微提高速度
                accelRate = 0;
            }

            #endregion


            //计算目标速度与当前速度之间的差值
            float speedDif = targetSpeed - fsm.owner.rigidbody.velocity.x;

            float movement = speedDif * accelRate;

            fsm.owner.rigidbody.AddForce(movement * Vector2.right, ForceMode2D.Force);
        }
    }
}