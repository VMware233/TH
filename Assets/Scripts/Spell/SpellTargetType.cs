using System;
using Sirenix.OdinInspector;

namespace TH.Spells
{
    [Flags]
    public enum SpellTargetType
    {
        None = 0,

        [LabelText("实体")]
        Entities = 1,

        [LabelText("方向")]
        Direction = 2,

        [LabelText("位置")]
        Position = 4,

        [LabelText("位置和方向")]
        DirectionAndPosition = Direction | Position,
    }
}