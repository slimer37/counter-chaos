using System;
using Interactables.Base;
using UnityEngine;

namespace Interactables.Holding
{
    public abstract class Item : MonoBehaviour, IInteractable
    {
        [field: SerializeField] public HoldableInfo Info { get; private set; }

        public bool IsHeld { get; private set; }

        public InteractionIcon Icon => InteractionIcon.Pickup;

        public bool CanInteract(Transform sender) => !IsHeld && !Inventory.Main.IsFull;
        public void OnInteract(Transform sender) => OnPickup(sender);

        protected virtual void Reset()
        {
            // Enable dropping/throwing by default.
            var info = Info;
            info.canBeDropped = info.canBeThrown = true;
            Info = info;
        }

        protected virtual void OnValidate()
        {
            if (!Info.canBeDropped && Info.canBeThrown)
            {
                Debug.LogWarning("Cannot be throwable if not droppable.", gameObject);
                
                var temp = Info;
                temp.canBeThrown = false;
                Info = temp;
            }
        }

        protected virtual void OnPickup(Transform sender)
        {
            if (!sender.CompareTag("Player"))
                throw new InvalidOperationException("Non-players cannot use items."); 
            
            if (IsHeld) return;

            if (Inventory.Main.TryGive(this)) Equip(sender);
        }

        public virtual void Drop() => Equip(null);

        internal virtual void Equip(Transform holder)
        {
            transform.parent = holder;
            IsHeld = (bool)holder;
        }
    }
}