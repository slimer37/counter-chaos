using System;
using Interactables.Base;
using UnityEngine;

namespace Interactables.Holding
{
    public sealed class Pickuppable : MonoBehaviour, IInteractHandler
    {
        [field: SerializeField] public bool Throwable { get; set; } = true;
        
        [Header("Overrides")]
        [SerializeField] Vector3 overrideHoldingPosition;
        [SerializeField] bool useRotationIfZeroes;
        [SerializeField] Vector3 overrideHoldingRotation;
        
        [Space(15)]
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

        void Reset()
        {
            TryGetComponent(out hoverable);
            TryGetComponent(out rb);
            TryGetComponent(out rend);
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

        public void OnInteract(Transform sender) => OnPickup(sender);

        void OnPickup(Transform sender)
        {
            var holder = sender.GetComponent<ItemHolder>();
            
            if (!holder && isHeld) throw new InvalidOperationException("NPC called OnInteract on held pickuppable.");
            
            if (isHeld || holder && holder.IsHoldingItem) return;
            
            if (holder)
            {
                var pos = UseIfNotZeroes(overrideHoldingPosition);
                var rot = useRotationIfZeroes ? overrideHoldingRotation : UseIfNotZeroes(overrideHoldingRotation);
                holder.Give(this, pos, rot);

                static Vector3? UseIfNotZeroes(Vector3 v) => v != Vector3.zero ? v : (Vector3?)null;
            }
            else
                Setup(sender);
        }

        internal void Drop() => Setup(null);

        internal void Toss(Vector3 direction, float force)
        {
            Drop();
            rb.AddForce(direction * force, ForceMode.Impulse);
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        void OnCollisionEnter(Collision other)
        {
            if (rb.collisionDetectionMode == CollisionDetectionMode.Continuous)
                rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
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
