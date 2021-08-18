using Interactables.Holding;
using UnityEngine;

namespace Furniture
{
    [RequireComponent(typeof(Pickuppable))]
    public class Shelf : MonoBehaviour
    {
        public enum Style { Gondola }
        
        [field: SerializeField] internal Style ShelfStyle { get; private set; }
    }
}
