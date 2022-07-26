using System;
using UnityEngine;

namespace Interactables.Holding
{
    public sealed class Pickuppable : Item
    {
        [SerializeField] bool useColliderBounds;
        [SerializeField, Tooltip("These are never used if useColliderBounds is false.")]
        BoxCollider[] boxColliders;
        
        [SerializeField] Rigidbody rb;
        [SerializeField] Renderer rend;

        bool nonPhysics;
        Bounds meshBounds;

        public float BoundHalfDiagonal { get; private set; }
        public float ToTopDistance { get; private set; }
        public float StandingDistance { get; private set; }

        void OnDrawGizmosSelected()
        {
            if (Application.isPlaying)
                Gizmos.DrawWireSphere(transform.position, BoundHalfDiagonal);
        }

        protected override void Reset()
        {
            base.Reset();
            
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

        public bool IsIntersecting(LayerMask mask)
        {
            if (!useColliderBounds)
                return Physics.CheckBox(transform.TransformPoint(meshBounds.center), meshBounds.extents, transform.rotation, mask);
            
            foreach (var c in boxColliders)
                if (Physics.CheckBox(transform.TransformPoint(c.center), c.size / 2, c.transform.rotation, mask))
                    return true;
            
            return false;
        }

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

        internal override void Equip(Transform holder)
        {
            base.Equip(holder);
            if (!nonPhysics) rb.isKinematic = holder;
        }
    }
}
