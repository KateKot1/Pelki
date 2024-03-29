using System;
using System.Collections.Generic;
using Pelki.Gameplay.Inputs;
using UnityEngine;

namespace Pelki.Gameplay.Puzzles
{
    public class CirclePuzzle : MonoBehaviour, IPuzzle
    {
        [SerializeField] private List<CirclePuzzleSegment> circleSegments;
        [SerializeField] private PuzzleInput puzzleInput;

        private bool isSolved = false;
        
        public event Action Solved;
        
        private void Awake()
        {
            puzzleInput.Swiped += OnSwiped;
        }
        
        private void Update() {
            if (isSolved) {
                return;
            }
            
            int solvedCount = 0;
            foreach (var segment in circleSegments) {
                if (segment.IsCorrectRotation()) {
                    solvedCount += 1;
                }
            }

            if (circleSegments.Count == solvedCount) {
                Debug.Log("Puzzle is solved");
                Solved?.Invoke();
                isSolved = true;
            }
        }

        private void OnSwiped(Vector2 direction, GameObject touchObject)
        {
            Debug.Log("OnSwiped");
            var segment = touchObject.GetComponent<CirclePuzzleSegment>();
            if (!segment.IsRotationEnabled)
            {
                segment.DoRotate(direction);
            }
        }
    }
}