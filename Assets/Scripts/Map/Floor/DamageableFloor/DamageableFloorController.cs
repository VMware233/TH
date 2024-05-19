using TH.Damage;

namespace TH.Map
{
    public class DamageableFloorController : FloorController, IDamageableController
    {
        IDamageable IDamageableController.damageable => floor as IDamageable;
    }
}
