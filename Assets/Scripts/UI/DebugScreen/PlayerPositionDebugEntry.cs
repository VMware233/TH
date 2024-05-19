using TH.Entities;
using VMFramework.UI;
using VMFramework.Core;
using VMFramework.GameLogicArchitecture;

namespace TH.UI
{
    [GamePrefabTypeAutoRegister(ID)]
    public class PlayerPositionDebugEntry : TitleContentDebugEntry
    {
        public const string ID = "player_position_debug_entry";

        public override bool ShouldDisplay()
        {
            return PlayerManager.isThisPlayerInitialized;
        }

        protected override string GetContent()
        {
            var pos = PlayerManager.GetThisPlayerController().transform.position.XY();

            return $"X: {pos.x.ToString(1)}, Y: {pos.y.ToString(1)}";
        }
    }
}
