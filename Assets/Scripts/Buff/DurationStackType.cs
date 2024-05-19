using Sirenix.OdinInspector;

namespace TH.Buffs
{
    public enum DurationStackType
    {
        [LabelText("持续时间直接相加")]
        SimpleStack,
        [LabelText("取较长的那个作为新的持续时间")]
        StackToLonger,
        [LabelText("取较短的那个作为持续时间")]
        StackToShorter,
        [LabelText("只有当新Buff的层数高时才相加持续时间")]
        StackWhileHavingHigherLevel,
        [LabelText("只有当新Buff的层数高时持续时间才取较长的一个")]
        StackToLongerWhileHavingHigherLevel
    }
}