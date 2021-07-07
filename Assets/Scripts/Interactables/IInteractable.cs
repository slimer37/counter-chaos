using UnityEngine;

namespace Interactables
{
    public interface IInteractable
    {
        public abstract void OnInteract(Transform player);
    }
}
