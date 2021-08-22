using Interactables.Holding;
using Interactables.Base;
using UnityEngine;

namespace Furniture
{
    [RequireComponent(typeof(Pickuppable))]
    public class Shelf : MonoBehaviour, IInteractHandler
    {
        public enum Style { Gondola, Refrigerator }
        
        [field: SerializeField] internal Style ShelfStyle { get; private set; }

        ShelfBase attachedBase;
        int attachmentIndex;
        
        Hoverable hoverable;

        int numColliding;

        void OnCollisionEnter(Collision other)
        {
            if (numColliding == 0) hoverable.enabled = false;
            numColliding++;
        }
        
        void OnCollisionExit(Collision other)
        {
            numColliding--;
            if (numColliding == 0) hoverable.enabled = true;
        }

        void Awake() => hoverable = GetComponent<Hoverable>();

        internal void Disable() => hoverable.enabled = false;
        internal void Enable() => hoverable.enabled = true;

        internal void AttachTo(ShelfBase shelfBase, int atIndex)
        {
            attachedBase = shelfBase;
            attachmentIndex = atIndex;
        }
        
        // Basically OnPickup
        public void OnInteract(Transform sender)
        {
            if (attachedBase)
            {
                attachedBase.Detach(attachmentIndex);
                attachedBase = null;
            }
        }
    }
}
