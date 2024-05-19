using System;
using TH.Map;
using VMFramework.GameLogicArchitecture;

namespace TH.Property
{
    public class PositionPropertyOfFloor : PropertyOfFloor
    {
        protected override void OnFloorChanged(Floor previous, Floor current)
        {

        }
    }

    [GamePrefabTypeAutoRegister(ID)]
    public class FloorPositionPropertyConfig : FloorPropertyConfig
    {
        public const string ID = "floor_position_property";

        public override Type gameItemType => typeof(PositionPropertyOfFloor);

        protected override string GetFloorValueString(
            Floor floor)
        {
            return $"X:{floor.tile.x}, Y:{floor.tile.y}";
        }
    }
}