using System.Collections.Generic;
using NaughtyAttributes;
using Pelki.Gameplay.Characters;
using UnityEngine;

namespace Pelki.DestroyableObjects
{
    public class Spiderweb : Entity
    {
        [SerializeField] private List<Rigidbody2D> rigidbody2dOnBones = new List<Rigidbody2D>();

        protected override void Die()
        {
            CutSelf();
        }

        private void CutSelf()
        {
            foreach (Rigidbody2D rigidbody2d in rigidbody2dOnBones)
            {
                rigidbody2d.simulated = true;
            }
        }

        [Button("Find all bones")]
        private void EditorEditAllChildRigidbodies()
        {
            rigidbody2dOnBones.Clear();

            EditorRecursiveEditRigidbodies(transform);
        }

        private void EditorRecursiveEditRigidbodies(Transform currentTransform)
        {
            Rigidbody2D rigitbody2d = currentTransform.GetComponent<Rigidbody2D>();

            if (rigitbody2d != null)
            {
                rigitbody2d.simulated = false;
                rigidbody2dOnBones.Add(rigitbody2d);
            }

            for (int i = 0; i < currentTransform.childCount; i++)
            {
                Transform childTransform = currentTransform.GetChild(i);
                EditorRecursiveEditRigidbodies(childTransform);
            }
        }
    }
}