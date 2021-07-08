using Core;
using UnityEngine;
using DG.Tweening;

namespace Interactables.Holding
{
    public class ItemHolder : MonoBehaviour
    {
        [SerializeField] Vector3 holdingPosition;
        [SerializeField] float pickupTime;
        [SerializeField] float tossForce;
        [SerializeField, Layer] int heldObjectLayer;

        Pickuppable heldItem;
        int tempLayer;

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(transform.TransformPoint(holdingPosition), Vector3.one * 0.5f);
        }

        void OnEnable() => Pickuppable.ItemPickedUp += OnPickup; 
        void OnDisable() => Pickuppable.ItemPickedUp -= OnPickup;

        void OnPickup(Pickuppable pickuppable)
        {
            heldItem = pickuppable;
            var go = heldItem.gameObject;
            tempLayer = go.layer;
            go.layer = heldObjectLayer;
            
            heldItem.transform.DOLocalMove(holdingPosition, pickupTime);
            heldItem.transform.DOLocalRotateQuaternion(Quaternion.identity, pickupTime);
        }

        void OnDrop() => Drop(false);
        void OnToss() => Drop(true);

        void Drop(bool toss)
        {
            if (!heldItem) return;
            
            // Finish tweens if still in progress.
            heldItem.transform.DOComplete();
            
            heldItem.gameObject.layer = tempLayer;
            
            if (toss)
                heldItem.Toss(transform.forward, tossForce);
            else
                heldItem.Drop();
        }
    }
}
