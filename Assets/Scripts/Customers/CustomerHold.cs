using System;
using System.Collections.Generic;
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
        [SerializeField, Min(0)] float dropHeight = 0.5f;

        List<Pickuppable> heldItems = new();

        public bool IsHoldingItem => heldItems.Count > 0;
        
        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0, 1, 0, 0.5f);
            Gizmos.DrawCube(transform.TransformPoint(holdingPosition), Vector3.one * 0.25f);
        }

        internal YieldInstruction Pickup(Pickuppable pickuppable)
        {
            heldItems.Add(pickuppable);
            pickuppable.OnInteract(transform);
            return pickuppable.transform.DOLocalMove(holdingPosition, holdAnimDuration).WaitForCompletion();
        }

        internal YieldInstruction Drop(Vector3 position, Vector3 rotation)
        {
            var heldItem = heldItems[0];
            if (!heldItem) throw new Exception("Drop called with no held item.");
            
            heldItems.Remove(heldItem);

            var sequence = DOTween.Sequence();

            var tempHeight = position.y + heldItem.VerticalExtent;
            
            position.y += heldItem.VerticalExtent + dropHeight;
            
            var prepare = heldItem.transform.DOMoveY(position.y, dropSpeed).SetSpeedBased();
            var horizontalMove = heldItem.transform.DOMove(position, dropSpeed).SetSpeedBased();
            
            sequence.Append(prepare);
            sequence.Append(horizontalMove);
            sequence.Join(heldItem.transform.DORotate(rotation, horizontalMove.Duration()));
            sequence.Append(heldItem.transform.DOMoveY(tempHeight, dropSpeed).SetSpeedBased());
            
            return sequence.OnComplete(heldItem.Drop).WaitForCompletion();
        }
    }
}
