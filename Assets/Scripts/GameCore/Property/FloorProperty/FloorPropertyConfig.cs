using System;
using TH.Map;
using VMFramework.Property;

namespace TH.Property
{
    public abstract class PropertyOfFloor : PropertyOfGameItem
    {
        protected Floor floor => (Floor)target;

        protected sealed override void OnTargetChanged(object previous, object current)
        {
            base.OnTargetChanged(previous, current);

            var previousFloor = (Floor)previous;
            var currentFloor = (Floor)current;

            OnFloorChanged(previousFloor, currentFloor);
        }

        protected abstract void OnFloorChanged(Floor previous, Floor current);
    }

    public abstract class FloorPropertyConfig : PropertyConfig
    {
        public override Type targetType => typeof(Floor);

        protected override string idPrefix => "floor";

        public sealed override string GetValueString(object target)
        {
            return GetFloorValueString((Floor)target);
        }

        protected abstract string GetFloorValueString(Floor floor);
    }
}
