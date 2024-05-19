namespace TH.Damage
{
    public static class DamageableUtility
    {
        public static void ReceiveDamagePacket(this IDamageable damageable, DamagePacket packet)
        {
            damageable.ProduceDamageResult(packet, out var result);
            damageable.ProcessDamageResult(result);
        }
    }
}