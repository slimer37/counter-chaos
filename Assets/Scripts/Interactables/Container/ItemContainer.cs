using System.Collections.Generic;
using System.Collections.ObjectModel;
using Interactables.Base;
using Interactables.Holding;
using Products;
using UnityEngine;
using UnityEngine.Events;

namespace Interactables.Container
{
    [RequireComponent(typeof(ContainerPositioner))]
    public class ItemContainer : MonoBehaviour, IInteractable, ISecondaryInteractable
    {
        public UnityEvent onOpen;
        public UnityEvent onClose;
        [SerializeField] bool cannotBeClosed;
        [SerializeField] List<Pickuppable> contents;
        [SerializeField] float acceptanceDelay;
        [SerializeField] ProductInfo filterProduct;

        ContainerPositioner positioner;
        Pickuppable pickuppable;
        
        List<Pickuppable> itemsWaiting = new();
        bool open;
        
        public ReadOnlyCollection<Pickuppable> Contents => contents.AsReadOnly();

        void Reset()
        {
            TryGetComponent(out positioner);
            TryGetComponent(out pickuppable);
        }

        void Awake()
        {
            positioner = GetComponent<ContainerPositioner>();
            if (cannotBeClosed) open = true;
            if (TryGetComponent(out pickuppable)) pickuppable.enabled = !open;
        }

        void Start() =>
            positioner.PlaceInPositions(contents.ConvertAll(p => p.transform).ToArray(), 0, false);

        public void InitContents(Pickuppable[] toStore)
        {
            if (contents != null) throw new System.InvalidOperationException(name + " is already initialized.");
            contents = new List<Pickuppable>();
            contents.AddRange(toStore);
        }

        bool IsContainable(Component obj) =>
            contents.Count < positioner.TotalPositions
            && obj.CompareTag("Product")
            && (!filterProduct || obj.GetComponent<ProductIdentifier>().productInfo == filterProduct)
            && !obj.TryGetComponent<ItemContainer>(out _);
        
        public bool CanInteract(Transform sender) => open;
        
        public void OnInteract(Transform sender)
        {
            var inventory = Inventory.Main;

            if (inventory.Holder.IsHoldingItem)
            {
                if (!IsContainable(inventory.Holder.HeldItem)) return;
                AddItem(inventory.ClearActiveSlot(), true);
            }
            else if (contents.Count > 0)
                RemoveItem(inventory);
        }

        void AddItem(Pickuppable item, bool wasInteraction)
        {
            positioner.PlaceInPosition(item.transform, contents.Count, true, wasInteraction);
            contents.Add(item);
        }

        void RemoveItem(Inventory inventory)
        {
            if (positioner.IsAnimating || inventory.IsFull) return;
            
            var i = contents.Count - 1;
            if (positioner.TryGiveToPlayer(i))
                contents.RemoveAt(i);
        }

        public void OnSecondaryInteract(Transform sender) => ToggleOpen();

        void ToggleOpen()
        {
            if (cannotBeClosed) return;
            
            open = !open;
            
            if (pickuppable) pickuppable.enabled = !open;
            
            if (open)
            {
                onOpen.Invoke();
                Invoke(nameof(AcceptWaitingItems), acceptanceDelay);
            }
            else
            {
                onClose.Invoke();
                CancelInvoke();
            }
        }

        void AcceptWaitingItems()
        {
            itemsWaiting.ForEach(p => AddItem(p, false));
            itemsWaiting.Clear();
        }

        void OnCollisionEnter(Collision other)
        {
            // Look for collisions coming down through the top of the container.
            if (other.GetContact(0).normal.y < -0.7f)
            {
                var p = other.transform.GetComponent<Pickuppable>();
                if (open)
                {
                    if (IsContainable(other.transform))
                        AddItem(p, false);
                }
                else
                    itemsWaiting.Add(p);
            }
        }

        void OnCollisionExit(Collision other)
        {
            if (itemsWaiting.Count == 0 || !other.transform.CompareTag("Product")) return;

            var p = other.transform.GetComponent<Pickuppable>();
            itemsWaiting.Remove(p);
        }
    }
}
