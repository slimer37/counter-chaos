using UnityEngine;
using DG.Tweening;

namespace Interactables.Holding
{
    public class ItemHolder : MonoBehaviour
    {
        [SerializeField] Vector3 holdingPosition;
        [SerializeField] float pickupTime;

        Pickuppable heldItem;

        void OnEnable() => Pickuppable.ItemPickedUp += OnPickup; 
        void OnDisable() => Pickuppable.ItemPickedUp -= OnPickup;

        void OnPickup(Pickuppable pickuppable)
        {
            heldItem = pickuppable;
            heldItem.transform.DOLocalMove(holdingPosition, pickupTime);
            heldItem.transform.DOLocalRotateQuaternion(Quaternion.identity, pickupTime);
        }

        void OnDrop() => heldItem.Drop();
    }
}
