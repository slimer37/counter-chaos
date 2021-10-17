using System;
using Interactables.Base;
using UnityEngine;

namespace Interactables.Holding
{
    public sealed class Pickuppable : MonoBehaviour, IInteractHandler
    {
        [field: SerializeField] public HoldableInfo Info { get; private set; }

        [Header("Bounds")]
        [SerializeField] bool useColliderBounds;
        [SerializeField] bool neverIntersects;
        
        [Header("Overrides")]
        [SerializeField] Vector3 overrideHoldingPosition;
        [SerializeField] bool useRotationIfZeroes;
        [SerializeField] Vector3 overrideHoldingRotation;
        
        [Space(15)]
        [SerializeField] Hoverable hoverable;
        [SerializeField] Rigidbody rb;
        [SerializeField] bool nonPhysics;
        [SerializeField] Renderer rend;

        Bounds meshBounds;
        bool isHeld;

        public float BoundHalfDiagonal { get; private set; }
        public float VerticalExtent { get; private set; }
        public Vector3? OverridePosition => UseIfNotZeroes(overrideHoldingPosition);
        public Vector3? OverrideRotation => useRotationIfZeroes ? overrideHoldingRotation : UseIfNotZeroes(overrideHoldingRotation);
        
        static Vector3? UseIfNotZeroes(Vector3 v) => v != Vector3.zero ? v : (Vector3?)null;

        void OnDrawGizmosSelected()
        {
            if (Application.isPlaying)
                Gizmos.DrawWireSphere(transform.position, BoundHalfDiagonal);
        }

        void OnValidate()
        {
            if (!Info.canBeDropped && Info.canBeThrown)
            {
                Debug.LogWarning("Cannot be throwable if not droppable.", gameObject);
                
                var temp = Info;
                temp.canBeThrown = false;
                Info = temp;
            }
        }

        void Reset()
        {
            // Enable dropping/throwing by default.
            var info = Info;
            info.canBeDropped = info.canBeThrown = true;
            Info = info;
            
            TryGetComponent(out hoverable);
            TryGetComponent(out rb);
            TryGetComponent(out rend);
        }

        void Awake()
        {
            if (useColliderBounds)
            {
                if (!TryGetComponent(out Collider col))
                    throw new Exception("Tried to fetch collider bounds but no collider was found.");
                meshBounds = col.bounds;
            }
            else
                meshBounds = rend.bounds;
            
            BoundHalfDiagonal = Mathf.Sqrt(meshBounds.extents.x * meshBounds.extents.x + meshBounds.extents.z * meshBounds.extents.z);
            VerticalExtent = transform.position.y - meshBounds.center.y + meshBounds.extents.y;
            hoverable.OnAttemptHover += OnAttemptHover;
        }

        // Can hover if the sender is not holding anything.
        bool OnAttemptHover(Transform sender) => !isHeld && !ItemHolder.Main.IsHoldingItem;

        public bool IsIntersecting(LayerMask mask, Collider[] results) =>
            !neverIntersects
            && Physics.OverlapBoxNonAlloc(rend.bounds.center, meshBounds.extents, results, transform.rotation, mask) > 0;

        public void OnInteract(Transform sender) => OnPickup(sender);

        void OnPickup(Transform sender)
        {
            var isPlayer = sender.CompareTag("Player");
            if (!isPlayer && isHeld)
                throw new InvalidOperationException("NPC called OnInteract on held pickuppable.");
            
            var holder = ItemHolder.Main;
            
            if (isHeld || isPlayer && holder.IsHoldingItem) return;
            
            if (isPlayer)
                holder.Give(this);
            else
                Setup(sender);
        }

        public void Drop() => Setup(null);

        internal void Toss(Vector3 direction, float force)
        {
            if (nonPhysics || !Info.canBeThrown)
                throw new Exception("Don't call Toss on non-physics or non-throwable objects.");
            
            Drop();
            rb.AddForce(direction * force, ForceMode.Impulse);
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        void OnCollisionEnter(Collision other)
        {
            if (nonPhysics) return;
            if (rb.collisionDetectionMode == CollisionDetectionMode.Continuous)
                rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        }

        internal void Setup(Transform holder)
        {
            transform.parent = holder;

            var pickingUp = (bool)holder;
            isHeld = pickingUp;
            if (!nonPhysics) rb.isKinematic = pickingUp;
        }
    }
}
