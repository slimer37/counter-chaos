using System;
using Core;
using Interactables.Base;
using Interactables.Holding;
using UnityEngine;

namespace Furniture
{
    public abstract class Receptacle<T> : MonoBehaviour, IInteractable
        where T : MonoBehaviour, IInteractable
    {
        [SerializeField] InteractionChannel channel;

        protected virtual void OnEnable() => channel.OnInteractRelease += FinishAdd;
        protected virtual void OnDisable() => channel.OnInteractRelease -= FinishAdd;
        
        protected abstract void StartAdding(T component);
        protected abstract void FinishAdd();

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