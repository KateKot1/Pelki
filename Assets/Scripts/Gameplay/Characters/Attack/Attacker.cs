using System;
using Pelki.Gameplay.Inputs;
using UnityEngine;

namespace Pelki.Gameplay.Characters.Attack
{
    public class Attacker : MonoBehaviour
    {
        [SerializeField] private ProjectileSpawner _projectileSpawner;
        [SerializeField] private float _attackCooldown;

        private IInput _input;
        private float _reloadCompletionTime;

        public event Action InvokedRangeAttack;

        private bool CanPerformRangedAttack => Time.time > _reloadCompletionTime;

        public void Construct(IInput input)
        {
            _input = input;
        }

        private void Update()
        {
            if (IsPerformingRangedAttack())
            {
                RangedAttack();
            }
        }

        private bool IsPerformingRangedAttack()
        {
            bool isCalled = _input.IsRangedAttacking;

            return isCalled && CanPerformRangedAttack;
        }

        private void RangedAttack()
        {
            _projectileSpawner.Shoot(transform.right);
            _reloadCompletionTime = Time.time + _attackCooldown;
            InvokedRangeAttack?.Invoke();
        }
    }
}