using Interactables.Base;
using Interactables.Holding;
using UnityEngine;

namespace Furniture
{
    public class ShelfBase : MonoBehaviour, IInteractHandler, IStopInteractHandler
    {
        [SerializeField] int maxShelves;
        [SerializeField] Vector3 shelfOffset;
        [SerializeField] float shelfSnapInterval;
        [SerializeField] Shelf.Style style;
        
        Shelf[] shelves;

        ItemHolder currentInteractor;
        Camera playerCamera;
        
        Shelf shelf;
        bool shelfIsBeingAttached;
        int tempLayer;
        
        int shelfIndex;

        int ignoreRaycastLayer;

        void Awake()
        {
            ignoreRaycastLayer = LayerMask.GetMask("Ignore Raycast");
            shelves = new Shelf[maxShelves];
            GetComponent<Hoverable>().OnAttemptHover =
                sender=> sender.GetComponent<ItemHolder>()?.HeldItem?.GetComponent<Shelf>() ?? false;
        }

        public void OnInteract(Transform sender)
        {
            var holder = sender.GetComponent<ItemHolder>();
            if (!holder || !holder.IsHoldingItem || !holder.HeldItem.TryGetComponent<Shelf>(out var heldShelf)) return;
            if (heldShelf.ShelfStyle != style) return;

            shelfIsBeingAttached = true;
            
            shelf = heldShelf;
            playerCamera = holder.PlayerCam;
            currentInteractor = holder;
            currentInteractor.TakeFrom();
            
            shelf.transform.parent = transform;
            tempLayer = shelf.gameObject.layer;
            shelf.gameObject.layer = ignoreRaycastLayer;
            shelf.transform.rotation = Quaternion.identity;
        }

        void Update()
        {
            if (!shelfIsBeingAttached) return;
            
            var shelfPos = transform.TransformPoint(shelfOffset);

            if (Physics.Raycast(playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f)), out var hit))
            {
                if (hit.transform != transform && hit.transform != shelf.transform)
                {
                    OnStopInteract(null);
                    return;
                }
                
                shelfPos.y = Mathf.RoundToInt((hit.point.y - transform.position.y) / shelfSnapInterval) * shelfSnapInterval;
            }

            shelf.transform.position = shelfPos;
        }

        public void OnStopInteract(Transform sender)
        {
            if (!shelfIsBeingAttached) return;
            
            shelves[shelfIndex] = shelf;
            shelfIsBeingAttached = false;
            currentInteractor = null;
            shelf.gameObject.layer = tempLayer;
        }
    }
}
