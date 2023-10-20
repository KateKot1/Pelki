using Pelki.Gameplay.Characters.Movements;
using Pelki.Gameplay.DamageSystem;
using Pelki.Gameplay.Input;
using UnityEngine;

namespace Pelki.Gameplay.Characters
{
    public class PlayerCharacter : Entity
    {
        [SerializeField] private GroundMover mover;
        [SerializeField] private ProjectileSpawner projectileSpawner;
        [SerializeField] private float attackCooldown;

        private float reloadCompletionTime;
        private bool canPerformRangedAttack = true;
        private IInput input;

        public void Construct(IInput input)
        {
            this.input = input;
            mover.Construct(input);
        }

        private void Update()
        {
            if (!canPerformRangedAttack && Time.time > reloadCompletionTime)
            {
                canPerformRangedAttack = true;
            }

            if (IsPerformingRangedAttack())
            {
                RangedAttack();
            }
        }

        private bool IsPerformingRangedAttack()
        {
            bool isCalled = input.IsRangedAttacking;

            return isCalled && canPerformRangedAttack;
        }

        private void RangedAttack()
        {
            projectileSpawner.Shoot(transform.right);
            canPerformRangedAttack = false;
            reloadCompletionTime = Time.time + attackCooldown;
        }
    }
}