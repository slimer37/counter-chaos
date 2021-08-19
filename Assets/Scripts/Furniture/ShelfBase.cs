using System;
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
        int availableSlots;

        ItemHolder currentInteractor;
        Camera playerCamera;
        Controls controls;
        
        Shelf shelfToAttach;
        bool shelfIsBeingAttached;
        Vector3 shelfAttachPos;
        
        int shelfIndex;

        void OnValidate() => shelfOffset.y = 0;
        
        void Awake()
        {
            controls = new Controls();
            controls.Gameplay.Interact.canceled += OnReleaseInteract;
            
            hoverable = GetComponent<Hoverable>();
            shelves = new Shelf[maxShelves];
            
            GetComponent<Hoverable>().OnAttemptHover =
                sender => sender.GetComponent<ItemHolder>()?.HeldItem?.GetComponent<Shelf>() ?? false;

            availableSlots = maxShelves;
            foreach (var shelf in GetComponentsInChildren<Shelf>())
            {
                if (!shelf.gameObject.activeSelf) continue;

                var shelfT = shelf.transform;
                var localPos = shelfT.localPosition;
                var index = GetShelfIndex(localPos.y);
                localPos.y = minShelfHeight + index * shelfSnapInterval;
                shelfT.localPosition = localPos;

                if (shelves[index]) throw new Exception($"Shelf at duplicate index ({index}).");
                
                shelf.AttachTo(this, index);
                shelves[index] = shelf;
                availableSlots--;
            }
        }

        void OnDestroy() => controls.Dispose();

        public void OnInteract(Transform sender)
        {
            if (availableSlots == 0) return;
            
            var holder = sender.GetComponent<ItemHolder>();
            if (!holder || !holder.IsHoldingItem || !holder.HeldItem.TryGetComponent<Shelf>(out var heldShelf)) return;
            if (heldShelf.ShelfStyle != style) return;
            
            shelfToAttach = heldShelf;

            var shelfT = shelfToAttach.transform;

            shelfIndex = -1;

            for (var i = 0; i < shelves.Length; i++)
            {
                if (!shelves[i])
                {
                    shelfIndex = i;
                    break;
                }
            }

            if (shelfIndex == -1) throw new Exception("No empty indices found.");
            
            playerCamera = holder.PlayerCam;
            currentInteractor = holder;
            currentInteractor.TakeFrom();
            
            shelfT.parent = transform;
            shelfT.localRotation = Quaternion.identity;
            shelfT.localPosition = shelfOffset + Vector3.up * (minShelfHeight + shelfIndex * shelfSnapInterval);
            
            shelfToAttach.Disable();
            shelfIsBeingAttached = true;
            hoverable.enabled = false;
            controls.Enable();
        }

        int GetShelfIndex(float atHeight) =>
            Mathf.Clamp(Mathf.RoundToInt((atHeight - minShelfHeight) / shelfSnapInterval), 0, maxShelves - 1);

        void Update()
        {
            if (!shelfIsBeingAttached) return;

            var requestedShelfIndex = 0;
            
            if (mainCollider.Raycast(playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f)), out var hit, attachDistance))
            {
                shelfAttachPos = shelfOffset;
                var height = hit.point.y - transform.position.y;
                requestedShelfIndex = GetShelfIndex(height);
                var snappedHeight = requestedShelfIndex * shelfSnapInterval;
                shelfAttachPos.y = minShelfHeight + snappedHeight;
            }
            else Attach();

            if (shelves[requestedShelfIndex]) return;
            shelfIndex = requestedShelfIndex;
            shelfToAttach.transform.localPosition = shelfAttachPos;
        }

        void OnReleaseInteract(InputAction.CallbackContext ctx) => Attach();

        void Attach()
        {
            if (!shelfIsBeingAttached) return;

            if (shelves[shelfIndex]) throw new Exception($"Requested shelf index ({shelfIndex}) is filled.");
            
            shelves[shelfIndex] = shelfToAttach;
            shelfToAttach.AttachTo(this, shelfIndex);
            shelfToAttach.Enable();
            hoverable.enabled = true;

            shelfIsBeingAttached = false;
            
            controls.Disable();
            availableSlots--;
        }

        internal void Detach(int index)
        {
            shelves[index] = null;
            availableSlots++;
        }
    }
}
