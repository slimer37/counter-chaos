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

        public static event Action<Pickuppable> ItemPickedUp;
        static event Action<bool> UpdateHover;

        static bool playerIsHoldingObject;

        void OnDrawGizmosSelected()
        {
            if (Application.isPlaying)
                Gizmos.DrawWireSphere(transform.position, BoundHalfDiagonal);
        }

        void Awake()
        {
            meshBounds = rend.bounds;
            BoundHalfDiagonal = Mathf.Sqrt(meshBounds.extents.x * meshBounds.extents.x + meshBounds.extents.z * meshBounds.extents.z);
        }

        void OnEnable() => UpdateHover += SetHoverable;
        void OnDisable() => UpdateHover -= SetHoverable;

        public bool IsIntersecting(LayerMask mask, Collider[] results) =>
            Physics.OverlapBoxNonAlloc(transform.position, meshBounds.extents, results, transform.rotation, mask) > 0;
        
        public void OnInteract(Transform player)
        {
            if (playerIsHoldingObject || isHeld) return;

            hoverable.OnHoverExit();
            Setup(player);
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
            playerIsHoldingObject = pickingUp;
            
            if (pickingUp)
                ItemPickedUp?.Invoke(this);

            UpdateHover?.Invoke(!pickingUp);
        }

        void SetHoverable(bool value) => hoverable.enabled = value;
    }
}
