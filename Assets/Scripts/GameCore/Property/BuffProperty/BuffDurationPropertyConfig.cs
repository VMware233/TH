using System;
using TH.Buffs;
using VMFramework.Core;
using VMFramework.GameLogicArchitecture;

namespace TH.Property
{
    public class DurationPropertyOfBuff : PropertyOfBuff
    {
        protected override void OnBuffChanged(Buff previous, Buff current)
        {
            if (previous != null)
            {
                previous.duration.OnValueChanged -= OnFloatValueChanged;
            }

            if (current != null)
            {
                current.duration.OnValueChanged += OnFloatValueChanged;
            }
        }
    }
    
    [GamePrefabTypeAutoRegister(ID)]
    public class BuffDurationPropertyConfig : BuffPropertyConfig
    {
        public const string ID = "buff_duration_property";

        public override Type gameItemType => typeof(DurationPropertyOfBuff);

        protected override string GetBuffValueString(Buff buff)
        {
            return $"{buff.duration.value.ClampMin(0):0.0}";
        }
    }
}