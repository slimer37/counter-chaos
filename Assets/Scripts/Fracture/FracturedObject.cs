using System;
using System.Collections;
using UnityEngine;

namespace Fracture
{
    public class FracturedObject : MonoBehaviour
    {
        [SerializeField] Rigidbody[] rigidbodies;
        [SerializeField] ParticleSystem particles;
        float delay;

        Vector3[] originalRbPositions;
        Quaternion[] originalRbRotations;

        void OnValidate()
        {
            foreach (Transform child in transform)
            {
                if (child.gameObject.layer != LayerMask.NameToLayer("Fracture Piece"))
                {
                    Debug.LogWarning($"All children of {name} must be on the Fracture Piece layer.");
                    return;
                }
            }
        }

        void Reset() => rigidbodies = GetComponentsInChildren<Rigidbody>();

        void Awake()
        {
            originalRbPositions = new Vector3[rigidbodies.Length];
            originalRbRotations = new Quaternion[rigidbodies.Length];
            for (var i = 0; i < rigidbodies.Length; i++)
            {
                originalRbPositions[i] = rigidbodies[i].transform.localPosition;
                originalRbRotations[i] = rigidbodies[i].transform.localRotation;
            }
        }

        internal void Explode(float force, float destroyDelay, Action<FracturedObject> disableCallback)
        {
            foreach (var rb in rigidbodies)
                rb.AddExplosionForce(force, transform.position, 0);
        
            delay = destroyDelay;
            particles?.Play();
            StartCoroutine(DelayedOut(disableCallback));
        }

        IEnumerator DelayedOut(Action<FracturedObject> disableCallback)
        {
            yield return new WaitForSeconds(delay);
            for (var i = 0; i < rigidbodies.Length; i++)
            {
                var rbT = rigidbodies[i].transform;
                rbT.localPosition = originalRbPositions[i];
                rbT.localRotation = originalRbRotations[i];
                rigidbodies[i].velocity = Vector3.zero;
            }
        
            gameObject.SetActive(false);
            disableCallback(this);
        }
    }
}
