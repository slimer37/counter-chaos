using System;
using Interactables.Base;
using UnityEngine;

namespace Interactables.Holding
{
    public class Pickuppable : MonoBehaviour, IInteractHandler
    {
        [field: SerializeField] public bool Throwable { get; private set; } = true;
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

        public virtual void OnInteract(Transform sender) => OnPickup(sender, Vector3.zero);

        protected void OnPickup(Transform sender, Vector3 overridePosition)
        {
            var holder = sender.GetComponent<ItemHolder>();
            
            if (!holder && isHeld) throw new InvalidOperationException("NPC called OnInteract on held pickuppable.");
            
            if (isHeld || holder && holder.IsHoldingItem) return;
            
            if (holder)
            {
                if (overridePosition != Vector3.zero)
                    holder.Give(this, overridePosition);
                else
                    holder.Give(this);
            }
            else
                Setup(sender);
        }

        internal void Drop() => Setup(null);

        internal void Toss(Vector3 direction, float force)
        {
            Drop();
            rb.AddForce(direction * force, ForceMode.Impulse);
        }

        internal void Setup(Transform holder)
        {
            transform.parent = holder;

            var pickingUp = (bool)holder;
            isHeld = pickingUp;
            rb.isKinematic = pickingUp;
            hoverable.enabled = !pickingUp;
        }
    }
}
