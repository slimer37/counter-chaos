using Interactables.Base;
using Interactables.Holding;
using UnityEngine;

namespace Furniture
{
    public abstract class Receptacle<T> : MonoBehaviour, IInteractable
        where T : MonoBehaviour, IInteractable
    {
        [SerializeField] InteractionChannel channel;

        protected bool IsAdding { get; private set; }

        protected virtual void OnEnable() => channel.OnInteractRelease += PostAdd;
        protected virtual void OnDisable() => channel.OnInteractRelease -= PostAdd;

        void PostAdd()
        {
            if (!IsAdding) return;
            FinishAdd();
        }
        
        protected virtual void StartAdding(T component)
        {
            IsAdding = true;
            channel.Activate();
        }

        protected virtual void FinishAdd()
        {
            if (!IsAdding) return;
            IsAdding = false;
            channel.Deactivate();
        }

        public abstract bool CanAccept(Transform sender, T component);
        
        bool IInteractable.CanInteract(Transform t) =>
            t.CompareTag("Player") &&
            Inventory.Main.Holder.IsHoldingItem &&
            Inventory.Main.Holder.HeldItem.TryGetComponent(out T component) &&
            CanAccept(t, component);

        void IInteractable.OnInteract(Transform t) =>
            StartAdding(Inventory.Main.Holder.HeldItem.GetComponent<T>());
    }
}