using System;
using TH.Buffs;
using VMFramework.Property;

namespace TH.Property
{
    public abstract class PropertyOfBuff : PropertyOfGameItem
    {
        protected sealed override void OnTargetChanged(object previous, object current)
        {
            base.OnTargetChanged(previous, current);

            var previousBuff = (Buff)previous;
            var currentBuff = (Buff)current;

            OnBuffChanged(previousBuff, currentBuff);
        }

        protected abstract void OnBuffChanged(Buff previous, Buff current);
    }

    public abstract class BuffPropertyConfig : PropertyConfig
    {
        public override Type targetType => typeof(Buff);

        protected override string idPrefix => "buff";

        public sealed override string GetValueString(object target)
        {
            return GetBuffValueString((Buff)target);
        }

        protected abstract string GetBuffValueString(Buff buff);
    }
}