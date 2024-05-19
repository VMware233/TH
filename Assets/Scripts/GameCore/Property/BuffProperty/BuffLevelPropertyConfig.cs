using System;
using TH.Buffs;
using VMFramework.GameLogicArchitecture;

namespace TH.Property
{
    public class LevelPropertyOfBuff : PropertyOfBuff
    {
        protected override void OnBuffChanged(Buff previous, Buff current)
        {
            if (previous != null)
            {
                previous.level.OnValueChanged -= OnIntValueChanged;
            }

            if (current != null)
            {
                current.level.OnValueChanged += OnIntValueChanged;
            }
        }
    }
    
    [GamePrefabTypeAutoRegister(ID)]
    public class BuffLevelPropertyConfig : BuffPropertyConfig
    {
        public const string ID = "buff_level_property";

        public override Type gameItemType => typeof(LevelPropertyOfBuff);

        protected override string GetBuffValueString(Buff buff)
        {
            return buff.level.value.ToString();
        }
    }
}