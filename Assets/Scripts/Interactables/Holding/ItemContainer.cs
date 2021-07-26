using System.Collections.Generic;
using System.Collections.ObjectModel;
using Interactables.Base;
using UnityEngine;
using UnityEngine.Events;

namespace Interactables.Holding
{
    [RequireComponent(typeof(ContainerPositioner), typeof(Pickuppable))]
    public class ItemContainer : MonoBehaviour, IInteractHandler, ISecondaryInteractHandler
    {
        public UnityEvent onOpen;
        public UnityEvent onClose;
        [SerializeField] List<Pickuppable> contents;
        [SerializeField] ContainerPositioner positioner;
        [SerializeField] Pickuppable pickuppable;
        
        bool open;
        
        public ReadOnlyCollection<Pickuppable> Contents => contents.AsReadOnly();

        void Reset()
        {
            TryGetComponent(out positioner);
            TryGetComponent(out pickuppable);
        }

        void Start() =>
            positioner.PlaceInPositions(contents.ConvertAll(p => p.transform).ToArray(), 0, false);

        public void InitContents(Pickuppable[] toStore)
        {
            if (contents != null) throw new System.InvalidOperationException(name + " is already initialized.");
            contents = new List<Pickuppable>();
            contents.AddRange(toStore);
        }
        
        public void OnInteract(Transform sender)
        {
            var holder = sender.GetComponent<ItemHolder>();
            if (!holder) return;
            
            if (!open) return;

            if (holder.IsHoldingItem)
                AddItem(holder.TakeFrom(), true);
            else if (contents.Count > 0)
                RemoveItem(holder);
        }

        void AddItem(Pickuppable item, bool wasInteraction)
        {
            if (contents.Count >= positioner.TotalPositions) return;
            positioner.PlaceInPosition(item.transform, contents.Count, true, wasInteraction);
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
            pickuppable.enabled = !open;
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
                AddItem(other.transform.GetComponent<Pickuppable>(), false);
        }
    }
}
