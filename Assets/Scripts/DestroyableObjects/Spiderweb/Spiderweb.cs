using System;
using System.Collections.Generic;
using NaughtyAttributes;
using Pelki.Gameplay.Characters;
using Unity.VisualScripting;
using UnityEngine;

namespace Pelki.DestroyableObjects.Spiderweb
{
    public class Spiderweb : Entity
    {
        [SerializeField] private List<Rigidbody2D> _rigidbody2DOnBones = new();
        [SerializeField] private float _inertia;
        [SerializeField] private float _angleLimit;
        [SerializeField] private float _ballastMass;

        protected override void Die()
        {
            CutSelf();
        }

        private void CutSelf()
        {
            foreach (Rigidbody2D rigidbody2d in _rigidbody2DOnBones)
            {
                rigidbody2d.simulated = true;
            }
        }

        public void AddHook(Transform boneTransform, Transform partTransform)
        {
            GameObject hook = new GameObject("Hook");
            Rigidbody2D hookRigidbody2D = hook.AddComponent<Rigidbody2D>();

            hookRigidbody2D.bodyType = RigidbodyType2D.Static;

            Instantiate(hook, partTransform);
            hook.transform.position = boneTransform.position;
        }
        private void AddBallast(Transform boneTransform, Transform partTransform)
        {
            GameObject ballast = new GameObject("Ballast");
            Rigidbody2D ballastRigidbody2D = ballast.AddComponent<Rigidbody2D>();
            ballast.AddComponent<HingeJoint2D>();

            ballastRigidbody2D.mass = _ballastMass;
            ballastRigidbody2D.simulated = false;

            SpriteRenderer spriteRenderer =
                partTransform.GetComponent<SpriteRenderer>();
            float x = spriteRenderer.sprite.bounds.extents.x;

            Vector3 bonePosition = boneTransform.position;
            Vector3 boneDirection =
                boneTransform.TransformDirection(Vector3.right);
            float boneX = bonePosition.x;
            float boneY = bonePosition.y;
            float y = boneDirection.y * (x - boneX) / bonePosition.x + boneY;
            Vector3 ballastPosition = new Vector3(x, y);
            Quaternion ballastRotation = boneTransform.rotation;

            Instantiate(ballast, ballastPosition, ballastRotation,
                partTransform);
        }

        private void ProcessAllBones(out Transform firstBone,
            out Transform lastBone, Transform partTransform)
        {
            _rigidbody2DOnBones.Clear();
            int bonesCount = partTransform.childCount;
            firstBone = partTransform.GetChild(0);
            lastBone = partTransform.GetChild(bonesCount - 1);

            for (int i = 0; i < bonesCount; i++)
            {
                Transform bone = partTransform.GetChild(i);
                Rigidbody2D boneRigidbody = bone.AddComponent<Rigidbody2D>();
                HingeJoint2D hingeJoint2D = bone.AddComponent<HingeJoint2D>();
                _rigidbody2DOnBones.Add(boneRigidbody);

                JointAngleLimits2D angleLimits2D = hingeJoint2D.limits;
                boneRigidbody.mass = (float)Math.Pow(_inertia, i);
                boneRigidbody.simulated = false;
                hingeJoint2D.useLimits = true;
                angleLimits2D.max = _angleLimit;
                angleLimits2D.min = -_angleLimit;
                hingeJoint2D.limits = angleLimits2D;

                if (i != 0)
                {
                    Rigidbody2D previousBone = partTransform.GetChild(i - 1)
                        .GetComponent<Rigidbody2D>();
                    hingeJoint2D.connectedBody = previousBone;
                }
            }
        }

        private void EditSpiderwebPart(Transform partTransform)
        {
            ProcessAllBones(out var firstBone, out var lastBone,
                partTransform);

            AddHook(firstBone, partTransform);
            AddBallast(lastBone, partTransform);
        }

        [Button("Build spiderweb")]
        private void EditorBuildSpiderweb()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform partTransform = transform.GetChild(i);
                EditSpiderwebPart(partTransform);
            }
        }
    }
}