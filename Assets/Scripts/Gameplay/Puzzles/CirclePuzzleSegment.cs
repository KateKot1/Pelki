using UnityEngine;

namespace Pelki.Gameplay.Puzzles
{
    public class CirclePuzzleSegment : MonoBehaviour
    {
        [SerializeField] private float correctRotationZ;
        [SerializeField] private float startRotationZ;
        [SerializeField] private float rotationAngleInDegrees;
        [SerializeField] private float rotationSpeed = 5.0f;
        
        private float currentRotationZ = 0.0f;
        private float futureRotationZ;
        private bool isRotationEnabled = false;

        public bool IsRotationEnabled => isRotationEnabled;
        
        private void Start() {
            transform.eulerAngles = new Vector3(0, 0, startRotationZ);
            currentRotationZ = transform.eulerAngles.z;
        }
        
        private void Update() {
            if (!isRotationEnabled) {
                return;
            }

            if (Mathf.Abs(transform.eulerAngles.z - futureRotationZ) > 0.01f)
            {
                Quaternion newQuaternion = Quaternion.Euler(0, 0, futureRotationZ);
                transform.rotation = Quaternion.Lerp(transform.rotation, newQuaternion, 
                    Time.deltaTime * rotationSpeed);
            } else {
                currentRotationZ = futureRotationZ;
                DisableRotation();
            }
        }
        
        public bool IsCorrectRotation()
        {
            return Mathf.Abs(currentRotationZ - correctRotationZ) < 0.01f;
        }

        public void DoRotate(Vector2 direction)
        {
            CalculateFutureRotationZ(direction);
            EnableRotation();
        }
        
        private void EnableRotation() 
        {
            isRotationEnabled = true;
        }

        private void DisableRotation() 
        {
            isRotationEnabled = false;
        }
        
        private void CalculateFutureRotationZ(Vector2 direction)
        {
            if (direction.x == 1)
            {
                futureRotationZ = currentRotationZ + rotationAngleInDegrees;
            }
            else
            {
                futureRotationZ = currentRotationZ - rotationAngleInDegrees;
            }
            if (Mathf.Approximately(futureRotationZ, 360)) {
                futureRotationZ = Mathf.Round(futureRotationZ - 360);
            }
        }
    }
}