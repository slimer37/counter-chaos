using System;
using UnityEngine;

namespace Interactables.Holding
{
    public sealed class Pickuppable : MonoBehaviour, IInteractable
    {
        [SerializeField] Hoverable hoverable;
        [SerializeField] Rigidbody rb;
        [SerializeField] Renderer boundingBoxSource;

        bool isHeld;
        Bounds bounds;

        public static event Action<Pickuppable> ItemPickedUp;

        static bool playerIsHoldingObject;

        void Awake() => bounds = boundingBoxSource.bounds;

        public bool IsIntersecting(LayerMask mask, Collider[] results, float extentIncrease) =>
            Physics.OverlapBoxNonAlloc(transform.position, bounds.extents + Vector3.one * extentIncrease,
                results, transform.rotation, mask) > 0;
        
        public void OnInteract(Transform player)
        {
            if (playerIsHoldingObject || isHeld) return;

            hoverable.OnHoverExit();
            Setup(player);
            ItemPickedUp?.Invoke(this);
        }

        public void Drop() => Setup(null);

        public void Toss(Vector3 direction, float force)
        {
            Drop();
            rb.AddForce(direction * force, ForceMode.Impulse);
        }

        void Setup(Transform holder)
        {
            transform.parent = holder;
            isHeld = holder;
            rb.isKinematic = holder;
            playerIsHoldingObject = holder;
            hoverable.enabled = !holder;
        }
    }
}
