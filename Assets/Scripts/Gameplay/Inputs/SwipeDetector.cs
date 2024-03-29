using System;
using UnityEngine;

namespace Pelki.Gameplay.Inputs
{
    public class SwipeDetector
    {
        private float deadZone = 50;
        private Vector2 _tapPosition;
        private Vector2 _swipeDelta;
        
        private bool _isSwiping;

        public Vector2 TapPosition => _tapPosition;
        public Vector2 SwipeDelta => _swipeDelta;

        public bool IsDetected()
        {
            if (Application.isEditor)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _isSwiping = true;
                    _tapPosition = Input.mousePosition;
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    ResetSwipe();
                }
            }
            else
            {
                if (Input.touchCount > 0)
                {
                    if (Input.GetTouch(0).phase == TouchPhase.Began)
                    {
                        _isSwiping = true;
                        _tapPosition = Input.GetTouch(0).position;
                    }
                    else if (Input.GetTouch(0).phase == TouchPhase.Canceled ||
                             Input.GetTouch(0).phase == TouchPhase.Ended)
                    {
                        ResetSwipe();
                    }
                }
            }
            
            return CheckSwipe();
        }
        
        public void ResetSwipe()
        {
            _isSwiping = false;
            _tapPosition = Vector2.zero;
            _swipeDelta = Vector2.zero;
        }
        
        private bool CheckSwipe()
        {
            _swipeDelta = Vector2.zero;

            if (_isSwiping)
            {
                if (Application.isEditor && Input.GetMouseButton(0))
                {
                    _swipeDelta = (Vector2)Input.mousePosition - _tapPosition;
                } else if (Input.touchCount > 0)
                {
                    _swipeDelta = Input.GetTouch(0).position - _tapPosition;
                }
            }

            return _swipeDelta.magnitude > deadZone;
        }
    }
}