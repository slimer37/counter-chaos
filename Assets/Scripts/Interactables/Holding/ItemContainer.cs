using System.Collections.Generic;
using System.Collections.ObjectModel;
using Interactables.Base;
using UnityEngine;

namespace Interactables.Holding
{
    public class ItemContainer : Pickuppable, ISecondaryInteractHandler
    {
        [SerializeField] Vector3 overrideHoldingPosition;
        [SerializeField] Animator animator;
        [SerializeField] List<Pickuppable> contents;
        
        bool open;
        static readonly int OpenState = Animator.StringToHash("open");
        static readonly int CloseState = Animator.StringToHash("close");
        
        public ReadOnlyCollection<Pickuppable> Contents => contents.AsReadOnly();

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

            if (contents.Count == 0) return;
            
            var holder = sender.GetComponent<ItemHolder>();
            if (!holder || holder.IsHoldingItem) return;

            var item = contents[0];
            item.gameObject.SetActive(true);
            item.transform.position = transform.position;
            holder.Give(item);
            contents.RemoveAt(0);
        }

        public void OnSecondaryInteract(Transform sender) => ToggleOpen();

        void ToggleOpen()
        {
            open = !open;
            animator.Play(open ? OpenState : CloseState);
        }
    }
}
