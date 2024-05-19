namespace TH.Entities
{
    public interface ICrouchActivatable
    {
        public void CrouchActivate(Player player);

        public void CrouchInActivate(Player player);
    }

    public interface ICrouchActivatableController
    {
        public ICrouchActivatable crouchActivatable { get; }
    }
}