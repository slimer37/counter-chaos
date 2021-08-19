using Interactables.Holding;
using Interactables.Base;
using UnityEngine;

namespace Furniture
{
    [RequireComponent(typeof(Pickuppable))]
    public class Shelf : MonoBehaviour, IInteractHandler
    {
        public enum Style { Gondola }
        
        [field: SerializeField] internal Style ShelfStyle { get; private set; }

        ShelfBase attachedBase;
        int attachmentIndex;
        
        Hoverable hoverable;

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
