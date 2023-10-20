using UnityEngine;

namespace Pelki.Gameplay.DamageSystem
{
    public class ProjectileSpawner : MonoBehaviour
    {
        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private Transform spawnPoint;

        public void Shoot(Vector2 direction)
        {
            Projectile projectile = Instantiate(projectilePrefab,
                spawnPoint.position,
                spawnPoint.rotation
            );

            projectile.Initialize(direction);
        }
    }
}