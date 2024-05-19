using Sirenix.OdinInspector;

namespace TH.Buffs
{
    public enum LevelStackType
    {
        [LabelText("直接叠加")]
        SimpleStack,
        [LabelText("当新Buff比原有Buff持续时间长才叠加")]
        StackWhileHavingLongerDuration,
        [LabelText("只有当新Buff持续时间较长才取较高的一个")]
        StackToHigherLevelWhileHavingLongerDuration
    }
}