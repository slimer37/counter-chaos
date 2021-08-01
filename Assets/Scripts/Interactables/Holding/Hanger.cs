using DG.Tweening;
using Interactables.Base;
using UnityEngine;

namespace Interactables.Holding
{
    public class Hanger : MonoBehaviour, IInteractHandler
    {
        [SerializeField] float animTime;
        [SerializeField] Collider disableCollider;
        
        Pickuppable hungItem;

        void Reset() => TryGetComponent(out disableCollider);

        public void OnInteract(Transform sender)
        {
            if (!sender.TryGetComponent(out ItemHolder holder)) return;

            if (!holder.IsHoldingItem)
            {
                if (hungItem)
                {
                    holder.Give(hungItem);
                    hungItem = null;
                }
                
                return;
            }
            
            var item = holder.HeldItem;
        
            if (!item.CanBeHung) return;

            holder.TakeFrom();

            hungItem = item;
            var itemT = item.transform;
            itemT.parent = transform;
            itemT.DOLocalMove(Vector3.zero, animTime);
            itemT.DOLocalRotateQuaternion(Quaternion.identity, animTime);
            
            Physics.IgnoreCollision(item.GetComponent<Collider>(), disableCollider);
            var rb = item.GetComponent<Rigidbody>();
            rb.isKinematic = true;
        }
    }
}
