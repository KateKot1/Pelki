using System;
using NaughtyAttributes;
using Pelki.Gameplay.DamageSystem;
using UnityEngine;

namespace Pelki.Gameplay.Characters
{
    public class Entity : MonoBehaviour, IDamageable
    {
        [MinValue(0)]
        [SerializeField] private int maxHealth;
        [Header("Debug")]
        [SerializeField] private bool isDebugLog;

        public int Health { get; private set; }

        protected virtual void Start()
        {
            Health = maxHealth;
        }

        public void TakeDamage(int damage)
        {
            if (damage < 0)
            {
                ThisDebugLogError("Taking damage", "Damage in negative range", this);
                return;
            }

            Health -= damage;
            if (Health <= 0)
            {
                Die();
            }

            ThisDebugLog("Taking damage", "Current health is " + Health.ToString(), this);
        }

        protected virtual void Die()
        {
            DestroySelf();
        }

        private void DestroySelf()
        {
            Destroy(gameObject);
        }

        private void ThisDebugLog(string actionName, string message, UnityEngine.Object context = null)
        {
            if (isDebugLog)
            {
                Debug.Log($"[{DateTime.Now:mm:ss:ffff}]: {nameof(Entity)}:[{actionName}] > {message}",
                    context);
            }
        }

        private void ThisDebugLogError(string actionName, string message, UnityEngine.Object context = null)
        {
            if (isDebugLog)
            {
                Debug.LogError($"[{DateTime.Now:mm:ss:ffff}]: {nameof(Entity)}:[{actionName}] > {message}",
                    context);
            }
        }
    }
}