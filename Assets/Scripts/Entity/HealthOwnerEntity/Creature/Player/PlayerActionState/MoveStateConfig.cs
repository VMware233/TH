using System;
using UnityEngine;
using VMFramework.GameLogicArchitecture;

namespace TH.Entities
{
    [GamePrefabTypeAutoRegister(ID)]
    public class MoveStateConfig : PlayerActionStateConfig
    {
        public const string ID = "move_player_state";

        public override Type gameItemType => typeof(MoveState);

        [Header("Run")]
        //public float runMaxSpeed; //我们希望玩家达到的目标速度。
        public float runAcceleration; //时间（大约）我们希望玩家从 0 加速到 runMaxSpeed 所需的时间。

        [HideInInspector] public float runAccelAmount; //施加到玩家的实际力量（乘以speedDiff）。
        public float runDecceleration; //我们希望玩家从 runMaxSpeed 加速到 0 所需的时间（大约）。
        [HideInInspector] public float runDeccelAmount;//施加到玩家的实际力（乘以speedDiff）。

        [Space(10)]
        [Range(0.01f, 1)] public float accelInAir;//乘数应用于空中时的加速度。
        [Range(0.01f, 1)] public float deccelInAir;

        public float lerpAmount;

        public bool doConserveMomentum;

        #region STATE PARAMETERS
        //变量控制玩家随时可以执行的各种动作。
        //这些字段可以是公共的，允许其他脚本读取它们
        //但只能私下写入。
        public bool IsFacingRight;
        public float LastOnGroundTime;
        #endregion
    }
}