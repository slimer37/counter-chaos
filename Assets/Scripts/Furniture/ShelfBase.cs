using Core;
using Interactables.Base;
using Interactables.Holding;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Furniture
{
    public class ShelfBase : MonoBehaviour, IInteractHandler
    {
        [SerializeField] Collider mainCollider;
        [SerializeField] float attachDistance;
        [SerializeField] int maxShelves;
        [SerializeField] Vector3 shelfOffset;
        [SerializeField] float minShelfHeight;
        [SerializeField] float shelfSnapInterval;
        [SerializeField] Shelf.Style style;

        Hoverable hoverable;
        
        Shelf[] shelves;

        ItemHolder currentInteractor;
        Camera playerCamera;
        Controls controls;
        
        Shelf shelf;
        bool shelfIsBeingAttached;
        Vector3 shelfAttachPos;
        
        int shelfIndex;
        Plane placementPlane;

        void Awake()
        {
            placementPlane = new Plane(transform.forward, transform.position);
            controls = new Controls();
            controls.Gameplay.Interact.canceled += OnReleaseInteract;
            
            hoverable = GetComponent<Hoverable>();
            shelves = new Shelf[maxShelves];
            GetComponent<Hoverable>().OnAttemptHover =
                sender => sender.GetComponent<ItemHolder>()?.HeldItem?.GetComponent<Shelf>() ?? false;
        }

        void OnDestroy() => controls.Dispose();

        public void OnInteract(Transform sender)
        {
            if (shelfIsBeingAttached)
            {
                Attach();
                return;
            }

            var holder = sender.GetComponent<ItemHolder>();
            if (!holder || !holder.IsHoldingItem || !holder.HeldItem.TryGetComponent<Shelf>(out var heldShelf)) return;
            if (heldShelf.ShelfStyle != style) return;

            shelfIsBeingAttached = true;
            
            shelf = heldShelf;
            playerCamera = holder.PlayerCam;
            currentInteractor = holder;
            currentInteractor.TakeFrom();
            
            shelf.Disable();
            hoverable.enabled = false;
            shelf.transform.parent = transform;
            shelf.transform.rotation = Quaternion.identity;
            shelfAttachPos = transform.TransformPoint(shelfOffset);
            shelfAttachPos.y = minShelfHeight;

            controls.Enable();
        }

        void Update()
        {
            if (!shelfIsBeingAttached) return;
            
            if (mainCollider.Raycast(playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f)), out var hit, attachDistance))
            {
                shelfAttachPos = transform.TransformPoint(shelfOffset);
                var height = hit.point.y - transform.position.y - minShelfHeight;
                var snappedHeight = Mathf.Clamp(Mathf.RoundToInt(height / shelfSnapInterval), 0, maxShelves - 1) * shelfSnapInterval;
                shelfAttachPos.y = minShelfHeight + snappedHeight;
            }
            else Attach();

            shelf.transform.position = shelfAttachPos;
        }

        void OnReleaseInteract(InputAction.CallbackContext ctx) => Attach();

        void Attach()
        {
            if (!shelfIsBeingAttached) return;
            
            shelves[shelfIndex] = shelf;
            shelf.Enable();
            hoverable.enabled = true;

            shelfIsBeingAttached = false;
            
            controls.Disable();
        }
    }
}
