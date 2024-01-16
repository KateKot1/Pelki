using System.Collections.Generic;
using Pelki.Gameplay.Camera;
using Pelki.Gameplay.Characters.Animations;
using Pelki.Gameplay.Characters.Attack;
using Pelki.Gameplay.Characters.Movements;
using Pelki.Gameplay.Input;
using Pelki.Gameplay.InventorySystem;
using Pelki.Gameplay.InventorySystem.Items;
using Pelki.Gameplay.SaveSystem;
using UnityEngine;

namespace Pelki.Gameplay.Characters
{
    public class PlayerCharacter : Entity, ICameraFollowByLookingAt
    {
        [SerializeField] private GroundMover _mover;
        [SerializeField] private PlayerAnimator _playerAnimator;
        [SerializeField] private Attacker _attacker;

        private IInput _input;
        private Transform _thisTransform;
        private InventoryProgress _inventoryProgress;
        private bool _isFacingRight = true;

        public Transform FollowRoot => _thisTransform;
        public InventoryProgress InventoryProgress => _inventoryProgress;
        public bool IsLookingRight => _isFacingRight;

        public void Construct(IInput input, InventoryProgress inventoryProgress)
        {
            //sttrox: кэширование transform, что бы избежать нативных вызовов Unity this.transform
            _thisTransform = transform;
            _input = input;
            _inventoryProgress = inventoryProgress;

            _mover.Construct(input);
            _attacker.Construct(input);

            _playerAnimator.Initialize();
        }

        private void OnEnable()
        {
            _attacker.InvokedRangeAttack += _playerAnimator.PlayRangedAttack;
        }

        private void OnDisable()
        {
            _attacker.InvokedRangeAttack -= _playerAnimator.PlayRangedAttack;
        }

        private void Update()
        {
            if (_input.Horizontal > 0f && !_isFacingRight)
            {
                _isFacingRight = true;
            }
            else if (_input.Horizontal < 0f && _isFacingRight)
            {
                _isFacingRight = false;
            }

            _playerAnimator.SetFlip(_input.Horizontal);
            _playerAnimator.SetState(_mover.CurrentState);
        }
    }
}