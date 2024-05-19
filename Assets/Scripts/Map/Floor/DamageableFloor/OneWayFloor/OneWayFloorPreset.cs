using System;

namespace TH.Map
{
    public class OneWayFloorPreset : DamageableFloorPreset
    {
        public override Type gameItemType => typeof(OneWayFloor);

        protected override Type floorControllerType => typeof(OneWayFloorController);
    }
}