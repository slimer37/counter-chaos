using System.Collections.Generic;
using System.Collections.ObjectModel;
using Interactables.Base;
using UnityEngine;
using UnityEngine.Events;

namespace Interactables.Holding
{
    [RequireComponent(typeof(ContainerPositioner))]
    public class ItemContainer : Pickuppable, ISecondaryInteractHandler
    {
        public UnityEvent onOpen;
        public UnityEvent onClose;
        [SerializeField] Vector3 overrideHoldingPosition;
        [SerializeField] List<Pickuppable> contents;
        [SerializeField] ContainerPositioner positioner;
        
        bool open;
        
        public ReadOnlyCollection<Pickuppable> Contents => contents.AsReadOnly();

        void Reset() => TryGetComponent(out positioner);

        void Start() =>
            positioner.PlaceInPositions(contents.ConvertAll(p => p.transform).ToArray(), 0, false);

        public void InitContents(Pickuppable[] toStore)
        {
            if (contents != null) throw new System.InvalidOperationException(name + " is already initialized.");
            contents = new List<Pickuppable>();
            contents.AddRange(toStore);
        }
        
        public override void OnInteract(Transform sender)
        {
            if (!open)
            {
                OnPickup(sender, overrideHoldingPosition);
                return;
            }

            var holder = sender.GetComponent<ItemHolder>();
            if (!holder) return;

            if (holder.IsHoldingItem)
                AddItem(holder.TakeFrom());
            else if (contents.Count > 0)
                RemoveItem(holder);
        }

        void AddItem(Pickuppable item)
        {
            if (contents.Count >= positioner.TotalPositions) return;
            positioner.PlaceInPosition(item.transform, contents.Count);
            contents.Add(item);
        }

        void RemoveItem(ItemHolder holder)
        {
            var i = contents.Count - 1;
            var item = contents[i];
            positioner.RestoreCollision(item);
            item.gameObject.SetActive(true);
            holder.Give(item);
            contents.RemoveAt(i);
        }

        public void OnSecondaryInteract(Transform sender) => ToggleOpen();

        void ToggleOpen()
        {
            open = !open;
            if (open)
                onOpen.Invoke();
            else
                onClose.Invoke();
        }

        void OnCollisionEnter(Collision other)
        {
            if (!open || !other.transform.CompareTag("Product")) return;
            
            // Look for collisions coming down through the top of the container.
            if (other.GetContact(0).normal.y < -0.7f)
                AddItem(other.transform.GetComponent<Pickuppable>());
        }
    }
}
