using DG.Tweening;
using Interactables.Base;
using UnityEngine;

namespace Interactables.Holding
{
    public class Hanger : MonoBehaviour, IInteractHandler
    {
        [SerializeField] float animTime;
        [SerializeField] Hoverable hoverable;
        
        Pickuppable hungItem;
        Collider itemCollider;

        void Reset() => TryGetComponent(out hoverable);

        void Awake() => hoverable.OnAttemptHover +=
            sender => Inventory.Main.Holder.IsHoldingItem && Inventory.Main.Holder.HeldItem.Info.canBeHung;

        public void OnInteract(Transform sender)
        {
            if (!sender.CompareTag("Player")) return;

            var inventory = Inventory.Main;

            if (!inventory.Holder.IsHoldingItem)
            {
                if (hungItem)
                {
                    if (!inventory.TryGive(hungItem)) return;
                    itemCollider.enabled = true;
                    hungItem = null;
                }
                
                return;
            }

            if (hungItem) return;
            
            var item = inventory.Holder.HeldItem;
        
            if (!item.Info.canBeHung) return;

            inventory.ClearActiveSlot();

            hungItem = item;
            var itemT = item.transform;
            itemT.parent = transform;
            itemT.DOLocalMove(Vector3.zero, animTime);
            itemT.DOLocalRotateQuaternion(Quaternion.identity, animTime);

            itemCollider = item.GetComponent<Collider>();
            itemCollider.enabled = false;
            var rb = item.GetComponent<Rigidbody>();
            rb.isKinematic = true;
        }
    }
}
