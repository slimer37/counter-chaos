using Interactables.Holding;
using Interactables.Base;
using UnityEngine;

namespace Furniture
{
    [RequireComponent(typeof(Pickuppable))]
    public class Shelf : MonoBehaviour
    {
        public enum Style { Gondola }
        
        [field: SerializeField] internal Style ShelfStyle { get; private set; }

        Hoverable hoverable;

        void Awake() => hoverable = GetComponent<Hoverable>();

        internal void Disable() => hoverable.enabled = false;
        internal void Enable() => hoverable.enabled = true;
    }
}
