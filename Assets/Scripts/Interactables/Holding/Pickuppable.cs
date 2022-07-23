using System;
using Interactables.Base;
using UnityEngine;

namespace Interactables.Holding
{
    public sealed class Pickuppable : MonoBehaviour, IInteractable
    {
        [field: SerializeField] public HoldableInfo Info { get; private set; }

        [Header("Bounds")]
        [SerializeField] bool useColliderBounds;
        [SerializeField, Tooltip("These are never used if useColliderBounds is false.")]
        BoxCollider[] boxColliders;
        
        [Header("Overrides")]
        [SerializeField] Vector3 overrideHoldingPosition;
        [SerializeField] bool useRotationIfZeroes;
        [SerializeField] Vector3 overrideHoldingRotation;
        
        [Space(15)]
        [SerializeField] Rigidbody rb;
        [SerializeField] Renderer rend;

        bool nonPhysics;
        Bounds meshBounds;
        bool isHeld;

        public bool IsHeld => isHeld;

        public float BoundHalfDiagonal { get; private set; }
        public float ToTopDistance { get; private set; }
        public float StandingDistance { get; private set; }
        public Vector3? OverridePosition => UseIfNotZeroes(overrideHoldingPosition);
        public Vector3? OverrideRotation => useRotationIfZeroes ? overrideHoldingRotation : UseIfNotZeroes(overrideHoldingRotation);
        
        static Vector3? UseIfNotZeroes(Vector3 v) => v != Vector3.zero ? v : null;

        public InteractionIcon Icon => InteractionIcon.Pickup;

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
            
            TryGetComponent(out rb);
            TryGetComponent(out rend);
        }

        void Awake()
        {
            // W/o rigidbody, treat as static (i.e. signs).
            nonPhysics = !rb;
            
            meshBounds = rend.localBounds;
            meshBounds.extents = Vector3.Scale(meshBounds.extents, transform.lossyScale);
            
            BoundHalfDiagonal = Mathf.Sqrt(meshBounds.extents.x * meshBounds.extents.x + meshBounds.extents.z * meshBounds.extents.z);
            StandingDistance = - meshBounds.center.y + meshBounds.extents.y;
            ToTopDistance = meshBounds.center.y + meshBounds.extents.y;
        }

        // Can hover if the sender is not holding anything.
        public bool CanInteract(Transform sender) => !isHeld && !Inventory.Main.IsFull;

        public bool IsIntersecting(LayerMask mask)
        {
            if (!useColliderBounds)
                return Physics.CheckBox(transform.TransformPoint(meshBounds.center), meshBounds.extents, transform.rotation, mask);
            
            foreach (var c in boxColliders)
                if (Physics.CheckBox(transform.TransformPoint(c.center), c.size / 2, c.transform.rotation, mask))
                    return true;
            
            return false;
        }

        public void OnInteract(Transform sender) => OnPickup(sender);

        void OnPickup(Transform sender)
        {
            var isPlayer = sender.CompareTag("Player");
            
            if (isHeld)
            {
                if (!isPlayer)
                    throw new InvalidOperationException("NPC called OnInteract on held pickuppable.");
                return;
            }
            
            if (isPlayer)
                Inventory.Main.TryGive(this);
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
