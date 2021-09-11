using System;
using Core;
using DG.Tweening;
using Interactables.Base;
using Interactables.Holding;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Furniture
{
    public class ShelfBase : MonoBehaviour, IInteractHandler
    {
        [SerializeField] Collider mainCollider;
        [SerializeField] Shelf.Style style;
        [SerializeField] float attachDistance;
        
        [Header("Shelf Position")]
        [SerializeField] int maxShelves;
        [SerializeField] float shelfForwardOffset;
        [SerializeField] float minShelfHeight;
        
        [Header("Attachment")]
        [SerializeField] float scaleX = 1;
        [SerializeField] float shelfSnapInterval;
        [SerializeField] float shelfPreviewOffset;
        
        [Header("Animation")]
        [SerializeField] float shelfInDuration;
        [SerializeField] float shelfAnimUp;
        [SerializeField] float beforeDownInterval;
        [SerializeField] float shelfDownDuration;

        Hoverable hoverable;
        
        Shelf[] shelves;
        int availableSlots;

        ItemHolder currentInteractor;
        Controls controls;
        
        Shelf shelfToAttach;
        bool shelfIsBeingAttached;
        Vector3 shelfAttachPos;
        
        int shelfIndex;
        int tempLayer;

        static int ignoreCollisionLayer;

        [RuntimeInitializeOnLoadMethod]
        static void Init() => ignoreCollisionLayer = LayerMask.NameToLayer("Ignore Collision");
        
        void Awake()
        {
            controls = new Controls();
            controls.Gameplay.Interact.canceled += OnReleaseInteract;
            
            hoverable = GetComponent<Hoverable>();
            shelves = new Shelf[maxShelves];
            
            GetComponent<Hoverable>().OnAttemptHover =
                sender => Vector3.Dot(transform.forward, sender.forward) < 0
                    && (ItemHolder.Main.HeldItem?.GetComponent<Shelf>() ?? false);

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

            if (!sender.CompareTag("Player")) return;
            
            var holder = ItemHolder.Main;
            if (!holder.IsHoldingItem || !holder.HeldItem.TryGetComponent<Shelf>(out var heldShelf)) return;
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
            
            currentInteractor = holder;
            currentInteractor.TakeFrom();
            
            // Set transform stuff after TakeFrom so shelf parent isn't reset.
            shelfT.parent = transform;
            shelfT.localRotation = Quaternion.identity;
            shelfT.localPosition = Vector3.forward * shelfPreviewOffset + Vector3.up * (minShelfHeight + shelfIndex * shelfSnapInterval);
            tempLayer = shelfToAttach.gameObject.layer;
            shelfToAttach.gameObject.layer = ignoreCollisionLayer;
            
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
            
            var playerCamera = Player.Camera;
            if (Vector3.Dot(transform.forward, playerCamera.transform.forward) < 0
                && mainCollider.Raycast(playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f)), out var hit, attachDistance))
            {
                shelfAttachPos = Vector3.forward * shelfPreviewOffset;
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

            var height = shelfToAttach.transform.localPosition.y;

            var attachmentSequence = DOTween.Sequence();
            attachmentSequence.Append(shelfToAttach.transform.DOLocalMoveZ(shelfForwardOffset, shelfInDuration));
            attachmentSequence.Join(shelfToAttach.transform.DOLocalMoveY(height + shelfAnimUp, shelfInDuration));
            attachmentSequence.Join(shelfToAttach.transform.DOScaleX(scaleX / transform.localScale.x, shelfInDuration));
            attachmentSequence.AppendInterval(beforeDownInterval);
            attachmentSequence.Append(shelfToAttach.transform.DOLocalMoveY(height, shelfDownDuration));
            attachmentSequence.SetId(shelfIndex | GetInstanceID());
            
            shelves[shelfIndex] = shelfToAttach;
            shelfToAttach.AttachTo(this, shelfIndex);
            shelfToAttach.Enable();
            shelfToAttach.gameObject.layer = tempLayer;
            hoverable.enabled = true;

            shelfIsBeingAttached = false;
            
            controls.Disable();
            availableSlots--;
        }

        internal void Detach(int index)
        {
            DOTween.Kill(index | GetInstanceID());
            shelves[index].transform.localScale = Vector3.one;
            shelves[index] = null;
            availableSlots++;
        }
    }
}