using System;
using DG.Tweening;
using Interactables.Holding;
using UnityEngine;

namespace Customers
{
    public class CustomerHold : MonoBehaviour
    {
        [SerializeField] Vector3 holdingPosition;
        [SerializeField] float holdAnimDuration;
        [SerializeField, Min(0.01f)] float dropSpeed = 1;

        Pickuppable heldItem;
        
        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0, 1, 0, 0.5f);
            Gizmos.DrawCube(transform.TransformPoint(holdingPosition), Vector3.one * 0.25f);
        }

        internal YieldInstruction Pickup(Pickuppable pickuppable)
        {
            (heldItem = pickuppable).OnInteract(transform);
            return pickuppable.transform.DOLocalMove(holdingPosition, holdAnimDuration).WaitForCompletion();
        }

        internal YieldInstruction Drop(Vector3 position)
        {
            if (!heldItem) throw new Exception("Drop called with no held item.");
            var temp = heldItem;
            position.y += heldItem.VerticalExtent;
            heldItem = null;
            return temp.transform.DOMove(position, dropSpeed).SetSpeedBased().OnComplete(temp.Drop).WaitForCompletion();
        }
    }
}
