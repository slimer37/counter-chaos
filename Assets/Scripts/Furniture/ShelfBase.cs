using System;
using Core;
using DG.Tweening;
using Interactables.Holding;
using UnityEngine;

namespace Furniture
{
    public class ShelfBase : Receptacle<Shelf>
    {
        [SerializeField] Collider mainCollider;
        [SerializeField] Shelf.Style style;
        [SerializeField] float attachDistance = 4;
        
        [Header("Shelf Position")]
        [SerializeField] int maxShelves;
        [SerializeField] float shelfForwardOffset;
        [SerializeField] float minShelfHeight;

        [Header("Attachment")]
        [SerializeField] bool ignoreDirection;
        [SerializeField] float scaleX = 1;
        [SerializeField] float shelfSnapInterval;
        [SerializeField] float shelfPreviewOffset = 0.8f;
        
        [Header("Animation")]
        [SerializeField] float shelfInDuration = 0.15f;
        [SerializeField] float shelfAnimUp = 0.1f;
        [SerializeField] float beforeDownInterval = 0.05f;
        [SerializeField] float shelfDownDuration = 0.05f;
        
        Shelf[] shelves;
        int availableSlots;
        
        Shelf shelfToAttach;
        Vector3 shelfAttachPos;
        
        int shelfIndex;
        int tempLayer;

        static int ignoreCollisionLayer;

        [RuntimeInitializeOnLoadMethod]
        static void Init() => ignoreCollisionLayer = LayerMask.NameToLayer("Ignore Collision");

        // Only interactable if no shelf is currently being attached and the player is in front,
        // holding a compatible shelf.
        public override bool CanAccept(Transform sender, Shelf component) =>
            availableSlots > 0
            && !IsAdding
            && (ignoreDirection || Vector3.Dot(transform.forward, sender.forward) < 0)
            && component.ShelfStyle == style;

        void Awake()
        {
            shelves = new Shelf[maxShelves];

            availableSlots = maxShelves;
            foreach (var shelf in GetComponentsInChildren<Shelf>())
            {
                if (!shelf.gameObject.activeSelf) continue;

                if (shelf.ShelfStyle != style) throw new Exception($"Shelf style of {shelf} does not match {name}.");

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

        protected override void StartAdding(Shelf shelf)
        {
            shelfToAttach = shelf;

            var shelfT = shelfToAttach.transform;

            // Find first null index.
            shelfIndex = Array.FindIndex(shelves, shelfSlot => !shelfSlot);

            if (shelfIndex == -1) throw new Exception("No empty indices found.");
            
            Inventory.Main.ClearActiveSlot();
            
            // Set transform stuff after TakeFrom so shelf parent isn't reset.
            shelfT.parent = transform;
            shelfT.localRotation = Quaternion.identity;
            shelfT.localPosition = GetLocalShelfPosition(shelfIndex);
            tempLayer = shelfToAttach.gameObject.layer;
            shelfToAttach.gameObject.layer = ignoreCollisionLayer;
            
            shelfToAttach.Disable();
            base.StartAdding(shelf);
        }

        Vector3 GetLocalShelfPosition(int atIndex) =>
            Vector3.forward * shelfPreviewOffset + Vector3.up * (minShelfHeight + atIndex * shelfSnapInterval);

        int GetShelfIndex(float atHeight) =>
            Mathf.Clamp(Mathf.RoundToInt((atHeight - minShelfHeight) / shelfSnapInterval), 0, maxShelves - 1);

        void Update()
        {
            if (!IsAdding) return;

            int requestedShelfIndex;
            
            var playerCamera = Player.Camera;
            if ((ignoreDirection || Vector3.Dot(transform.forward, playerCamera.transform.forward) < 0)
                && mainCollider.Raycast(playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f)), out var hit, attachDistance))
            {
                var height = hit.point.y - transform.position.y;
                requestedShelfIndex = GetShelfIndex(height);
                shelfAttachPos = GetLocalShelfPosition(requestedShelfIndex);
            }
            else
            {
                FinishAdd();
                return;
            }

            if (shelves[requestedShelfIndex]) return;
            shelfIndex = requestedShelfIndex;
            shelfToAttach.transform.localPosition = shelfAttachPos;
        }

        protected override void FinishAdd()
        {
            if (!IsAdding) return;
            base.FinishAdd();
            Attach();
        }

        void Attach()
        {
            if (shelves[shelfIndex]) throw new Exception($"Requested shelf index ({shelfIndex}) is filled.");

            var height = shelfToAttach.transform.localPosition.y;

            DOTween.Sequence()
                .Append(shelfToAttach.transform.DOLocalMoveZ(shelfForwardOffset, shelfInDuration))
                .Join(shelfToAttach.transform.DOLocalMoveY(height + shelfAnimUp, shelfInDuration))
                .Join(shelfToAttach.transform.DOScaleX(scaleX / transform.localScale.x, shelfInDuration))
                .AppendInterval(beforeDownInterval)
                .Append(shelfToAttach.transform.DOLocalMoveY(height, shelfDownDuration))
                .SetId(shelfIndex | GetInstanceID());
            
            shelves[shelfIndex] = shelfToAttach;
            shelfToAttach.AttachTo(this, shelfIndex);
            shelfToAttach.Enable();
            shelfToAttach.gameObject.layer = tempLayer;
            
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
