using System;
using NaughtyAttributes;
using Pelki.Interfaces;
using UnityEngine;

namespace Pelki.Gameplay.DamageSystem
{
    [RequireComponent(typeof(Collider2D))]
    public class Damager : MonoBehaviour
    {
        [MinValue(0)]
        [SerializeField] private int damage;
        [SerializeField] private LayerMask layerMaskDamage;

        public event Action<int> AttackPerformed;

        private void OnTriggerEnter2D(Collider2D otherCollider2D)
        {
            int otherLayer = otherCollider2D.gameObject.layer;

            if (IsDamageLayer(otherLayer))
            {
                if (otherCollider2D.TryGetComponent(out IDamageable damageable))
                {
                    DoDamage(damageable);
                }
            }

            AttackPerformed?.Invoke(otherLayer);

            bool IsDamageLayer(int otherLayer)
            {
                return (layerMaskDamage & (1 << otherLayer)) != 0;
            }
        }

        private void DoDamage(IDamageable damageable)
        {
            damageable.TakeDamage(damage);
        }
    }
}