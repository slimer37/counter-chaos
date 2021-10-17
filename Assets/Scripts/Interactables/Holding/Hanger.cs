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
            sender => ItemHolder.Main.IsHoldingItem && ItemHolder.Main.HeldItem.Info.canBeHung;

        public void OnInteract(Transform sender)
        {
            if (!sender.CompareTag("Player")) return;

            var holder = ItemHolder.Main;

            if (!holder.IsHoldingItem)
            {
                if (hungItem)
                {
                    holder.Give(hungItem);
                    itemCollider.enabled = true;
                    hungItem = null;
                }
                
                return;
            }

            if (hungItem) return;
            
            var item = holder.HeldItem;
        
            if (!item.Info.canBeHung) return;

            holder.TakeFrom();

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
