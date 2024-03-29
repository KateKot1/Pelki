using System;
using Pelki.Gameplay.Inputs;
using UnityEngine;

namespace Pelki.Gameplay.Characters.Movements
{
    public class GroundMover : MonoBehaviour
    {
        [Header("Move")]
        [SerializeField] private float _movementThreshold;
        [SerializeField] private float _movementSpeed;
        [SerializeField] private AnimationCurve _speedAccelerationCurve;
        [SerializeField] private float _accelerationTime;
        [SerializeField] private AnimationCurve _speedBreakingCurve;
        [SerializeField] private float _brakingTime;

        [Header("Jump and gravity specifics")]
        [SerializeField] private float _jumpVelocity = 12f;

        [SerializeField] private float _gravityFactor;

        [Header("Other")]
        [SerializeField] private LayerMask _groundMask;

        [SerializeField] private float _radiusGroundCheck;
        [SerializeField] private Rigidbody2D _rigidBody;

        [Header("Debug")]
        [SerializeField] private bool _isDrawDebug;

        [SerializeField] private Color _checkGroundDebugRayColor;

        //for Andrey: обрати внимание что сделал буффер для оптимизации памяти, что бы не выделять постоянно при рейкасте
        private readonly Collider2D[] _raycastBufferGroundCheckColliders = new Collider2D[1];
        private readonly RaycastHit2D[] _raycastBufferRaycastHit2D = new RaycastHit2D[1];

        private IInput _input;

        private bool _isGrounded;
        private bool _isJumping;

        private float _elapsedTime;
        private float _brakingElapsedTime;
        private MoverState _currentState;

        public MoverState CurrentState => _currentState;
        
        private bool IsInputJump => _input.IsJump;
        private bool IsIdle => _rigidBody.velocity == Vector2.zero;

        public void Construct(IInput input)
        {
            _input = input;
        }

        //sttrox: точно ли Fixed Update?
        private void FixedUpdate()
        {
            _isGrounded = CheckGrounded();

            DoMove();
            TryDoJump();

            ApplyGravity();

            CalculateCurrentState();
        }

        private bool CheckGrounded()
        {
            //todo sttrox: вынести кэш (Vector2)transform.position в начало кадра
            Vector2 colliderGroundPoint = transform.position;

            int countGroundCollisions = Physics2D.OverlapCircleNonAlloc(colliderGroundPoint, _radiusGroundCheck,
                _raycastBufferGroundCheckColliders, _groundMask);

            if (_isDrawDebug)
            {
                Debug.DrawRay(colliderGroundPoint, Vector2.down * _radiusGroundCheck, _checkGroundDebugRayColor);
            }

            bool result = countGroundCollisions > 0;

            return result;
        }

        private void ApplyGravity()
        {
            Vector3 gravity = Vector3.zero;

            gravity = Vector3.down * _gravityFactor * -Physics.gravity.y;

            _rigidBody.AddForce(gravity);
        }

        private void DoMove()
        {
            float rawHorizontalInput = _input.RawHorizontal;
            float absRawHorizontalInput = Mathf.Abs(rawHorizontalInput);
            Vector2 currentVelocity = _rigidBody.velocity;

            if (absRawHorizontalInput < _movementThreshold)
            {
                _brakingElapsedTime += Time.deltaTime;
                var percentBrakingTime = 1 - (_brakingElapsedTime / _brakingTime);

                float brakingClamping = _speedBreakingCurve.Evaluate(percentBrakingTime);
                Vector2 newVelocity = new Vector2(currentVelocity.x * brakingClamping, currentVelocity.y);
                _rigidBody.velocity = newVelocity;

                _elapsedTime = 0f;
            }
            else
            {
                _elapsedTime += Time.deltaTime;
                var percentTime = _elapsedTime / _accelerationTime;

                float speedClamping = _speedAccelerationCurve.Evaluate(percentTime);
                var speedFactor = Mathf.Min(speedClamping, absRawHorizontalInput);
                speedFactor *= Math.Sign(rawHorizontalInput);

                Vector2 newVelocity = new Vector2(speedFactor * _movementSpeed, currentVelocity.y);
                _rigidBody.velocity = newVelocity;

                _brakingElapsedTime = 0f;
            }
        }

        private bool TryDoJump()
        {
            if (IsInputJump && _isGrounded)
            {
                _rigidBody.velocity += Vector2.up * _jumpVelocity;
                _isJumping = true;
            }

            if (IsFalling())
            {
                _isJumping = false;
            }

            return _isJumping;

            bool IsFalling()
            {
                return _rigidBody.velocity.y < 0;
            }
        }

        private void CalculateCurrentState()
        {
            if (_isGrounded)
            {
                _currentState = IsIdle ? MoverState.Idle : MoverState.Run;
            }

            if (!_isGrounded && _isJumping)
            {
                _currentState = MoverState.Rise;
            }

            if (!_isGrounded && !_isJumping)
            {
                _currentState = MoverState.Fall;
            }
        }
    }
}