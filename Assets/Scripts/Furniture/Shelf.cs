using DG.Tweening;
using Interactables.Holding;
using Interactables.Base;
using UnityEngine;

namespace Furniture
{
    [RequireComponent(typeof(Pickuppable))]
    public class Shelf : MonoBehaviour, IInteractable
    {
        public enum Style { Gondola, Refrigerator, Warehouse }
        
        [field: SerializeField] internal Style ShelfStyle { get; private set; }
        [SerializeField] Transform meshTransform;

        ShelfBase attachedBase;
        int attachmentIndex;
        int numColliding;
        bool interactable = true;
        
        Tween shake;

        void OnCollisionEnter(Collision other) => numColliding++;

        void OnCollisionExit(Collision other) => numColliding--;

        public bool CanInteract(Transform s) => interactable;

        internal void Disable() => interactable = false;
        internal void Enable() => interactable = true;

        internal void AttachTo(ShelfBase shelfBase, int atIndex)
        {
            attachedBase = shelfBase;
            attachmentIndex = atIndex;
        }
        
        // Basically OnPickup
        public void OnInteract(Transform sender)
        {
            if (numColliding > 0)
            {
                shake?.Complete();
                shake = meshTransform.DOShakeRotation(0.4f, new Vector3(0.5f, 2, 0.5f), 15);
                return;
            }
            
            if (attachedBase)
            {
                attachedBase.Detach(attachmentIndex);
                attachedBase = null;
            }
        }

        public bool OnProcessInteract(Transform sender)
        {
            if (!CanInteract(sender)) return false;
            OnInteract(sender);
            if (numColliding > 0) return false;
            return true;
        }
    }
}
