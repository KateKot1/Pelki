using NaughtyAttributes;
using Pelki.Gameplay.Camera;
using Pelki.Gameplay.Characters.Animations;
using Pelki.Gameplay.Characters.Attack;
using Pelki.Gameplay.Characters.Movements;
using Pelki.Gameplay.Inputs;
using Pelki.Gameplay.Inventories;
using Pelki.Gameplay.Inventories.Items;
using UnityEngine;

namespace Pelki.Gameplay.Characters
{
    public class PlayerCharacter : Entity, ICameraFollowByLookingAt
    {
        [SerializeField] private GroundMover _mover;
        [SerializeField] private PlayerAnimator _playerAnimator;
        [SerializeField] private Attacker _attacker;
        [SerializeField] private TriggerDetector _itemTriggerDetector;

        private IInput _input;
        private Transform _thisTransform;
        private Inventory _inventory;
        private bool _isFacingRight = true;

        public Transform FollowRoot => _thisTransform;
        public bool IsLookingRight => _isFacingRight;

        [ShowNativeProperty] private string Editor_Inventory => _inventory.ToString();

        public void Construct(IInput input, Inventory inventory)
        {
            //sttrox: кэширование transform, что бы избежать нативных вызовов Unity this.transform
            _thisTransform = transform;
            _input = input;
            _inventory = inventory;

            _mover.Construct(input);
            _attacker.Construct(input);

            _playerAnimator.Initialize();
            _itemTriggerDetector.Detected += OnDetectedItem;
        }

        private void OnDetectedItem(GameObject item)
        {
            if (item.TryGetComponent<IItem>(out var detectedItem))
            {
                _inventory.AddItem(detectedItem);
                detectedItem.Destroy();
            }
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