using System;
using UnityEngine;

namespace Interactables.Holding
{
    public sealed class Pickuppable : MonoBehaviour, IInteractable
    {
        [SerializeField] Hoverable hoverable;
        [SerializeField] Rigidbody rb;
        [SerializeField] Renderer rend;

        Bounds meshBounds;
        bool isHeld;

        public float BoundHalfDiagonal { get; private set; }
        public float VerticalExtent => rend.bounds.extents.y;

        void OnDrawGizmosSelected()
        {
            if (Application.isPlaying)
                Gizmos.DrawWireSphere(transform.position, BoundHalfDiagonal);
        }

        void Awake()
        {
            meshBounds = rend.bounds;
            BoundHalfDiagonal = Mathf.Sqrt(meshBounds.extents.x * meshBounds.extents.x + meshBounds.extents.z * meshBounds.extents.z);
            hoverable.OnAttemptHover += OnAttemptHover;
        }

        // Can hover if the sender is not holding anything.
        bool OnAttemptHover(Transform sender) => !sender.GetComponent<ItemHolder>().IsHoldingItem;

        public bool IsIntersecting(LayerMask mask, Collider[] results) =>
            Physics.OverlapBoxNonAlloc(transform.position, meshBounds.extents, results, transform.rotation, mask) > 0;
        
        public void OnInteract(Transform sender)
        {
            var holder = sender.GetComponent<ItemHolder>();
            
            if (!holder && isHeld) throw new InvalidOperationException("NPC called OnInteract on held pickuppable.");
            
            if (isHeld || holder && holder.IsHoldingItem) return;
            
            Setup(sender);

            holder?.OnPickup(this);
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

            var pickingUp = (bool)holder;
            isHeld = pickingUp;
            rb.isKinematic = pickingUp;
            hoverable.enabled = !pickingUp;
        }
    }
}
