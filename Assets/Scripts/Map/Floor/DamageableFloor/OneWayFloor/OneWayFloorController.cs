using TH.Entities;

namespace TH.Map
{
    public class OneWayFloorController : DamageableFloorController, ICrouchActivatableController
    {
        ICrouchActivatable ICrouchActivatableController.crouchActivatable =>
            floor as ICrouchActivatable;
    }
}
