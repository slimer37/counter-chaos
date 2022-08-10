using Interactables.Base;
using Interactables.Holding;
using UnityEngine;

namespace Furniture
{
    public interface IReceptacle<in T> : IInteractable where T : MonoBehaviour, IInteractable
    {
        public void StartAdding(T obj);

        public bool CanAccept(Transform sender, T component) => component.TryGetComponent<T>(out _);
        
        bool IInteractable.CanInteract(Transform t) =>
            t.CompareTag("Player") &&
            Inventory.Main.Holder.IsHoldingItem &&
            Inventory.Main.Holder.HeldItem.TryGetComponent(out T component) &&
            CanAccept(t, component);

        void IInteractable.OnInteract(Transform t) =>
            StartAdding(Inventory.Main.Holder.HeldItem.GetComponent<T>());
    }
}