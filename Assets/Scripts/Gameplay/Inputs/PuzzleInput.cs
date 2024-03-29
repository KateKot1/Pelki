using System;
using UnityEngine;

namespace Pelki.Gameplay.Inputs
{
    public class PuzzleInput : MonoBehaviour
    {
        [SerializeField] private LayerMask layerMask;

        private SwipeDetector _swipeDetector;
        
        public event Action<Vector2, GameObject> Swiped;

        private void Awake()
        {
            _swipeDetector = new SwipeDetector();
        }

        private void Update()
        {
            if (_swipeDetector.IsDetected())
            {
                var pos = UnityEngine.Camera.main.ScreenToWorldPoint(_swipeDetector.TapPosition);
                RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
                if (hit != null && Mathf.Abs(_swipeDetector.SwipeDelta.x) > Mathf.Abs(_swipeDetector.SwipeDelta.y))
                {
                    GameObject touchedObject = hit.transform.gameObject;
                    if (touchedObject && (layerMask & (1 << touchedObject.gameObject.layer)) != 0)
                    {
                        Vector2 direction = _swipeDetector.SwipeDelta.x > 0 ? Vector2.right : Vector2.left;
                        Swiped.Invoke(direction, touchedObject);
                    }
                }
                _swipeDetector.ResetSwipe();
            }
        }
    }
}