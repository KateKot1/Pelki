using NaughtyAttributes;
using UnityEngine;

namespace Pelki.Gameplay.DamageSystem
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rigidbody2d;
        [Min(0f)]
        [SerializeField] private float speed;
        [MinValue(0)]
        [SerializeField] private int hitsLimit;
        [SerializeField] private LayerMask layerMaskDestroy;
        [SerializeField] private Damager damager;

        private int hitsUntilDestroy;
        private Vector2 direction;

        public void Initialize(Vector2 attackDirection)
        {
            direction = attackDirection;
            rigidbody2d.velocity = direction * speed;
        }

        private void OnEnable()
        {
            damager.AttackPerformed += CheckAttackedLayer;
        }

        private void Start()
        {
            hitsUntilDestroy = hitsLimit;
        }

        private void OnDisable()
        {
            damager.AttackPerformed -= CheckAttackedLayer;
        }

        private void CheckAttackedLayer(int otherLayer)
        {
            if (IsDestroyLayer(otherLayer))
            {
                DecreaseRemainingHits();
            }

            bool IsDestroyLayer(int otherLayer)
            {
                return (layerMaskDestroy & (1 << otherLayer)) != 0;
            }

            void DecreaseRemainingHits()
            {
                hitsUntilDestroy--;

                if (hitsUntilDestroy <= 0)
                {
                    DestroySelf();
                }
            }
        }

        private void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}