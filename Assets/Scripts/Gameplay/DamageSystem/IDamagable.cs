namespace Pelki.Interfaces
{
    public interface IDamageable
    {
        int Health { get; }

        void TakeDamage(int damage);
    }
}