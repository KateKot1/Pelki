namespace Pelki.Gameplay.DamageSystem
{
    public interface IDamageable
    {
        int Health { get; }

        void TakeDamage(int damage);
    }
}