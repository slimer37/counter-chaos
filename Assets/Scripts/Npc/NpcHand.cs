using System;
using System.Collections.Generic;
using DG.Tweening;
using Interactables.Holding;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Customers
{
    public class NpcHand : MonoBehaviour
    {
        [SerializeField] Vector3 leftHoldingPosition;
        [SerializeField] Vector3 rightHoldingPosition;
        [SerializeField] Transform leftHandTarget;
        [SerializeField] Transform rightHandTarget;
        [SerializeField, Min(0.01f)] float dropSpeed = 1;
        [SerializeField, Min(0)] float dropHeight = 0.5f;
        [SerializeField, Min(0)] float holdTime = 1;

        List<Pickuppable> heldItems = new();

        public bool IsHoldingItem => heldItems.Count > 0;
        
        void OnDrawGizmosSelected()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = new Color(0, 1, 0, 0.5f);
            Gizmos.DrawCube(leftHoldingPosition, Vector3.one * 0.25f);
            Gizmos.DrawCube(rightHoldingPosition, Vector3.one * 0.25f);
        }

        public YieldInstruction Pickup(Pickuppable pickuppable)
        {
            heldItems.Add(pickuppable);
            pickuppable.OnInteract(transform);
            
            var leftHanded = Random.value > 0.5f;
            var pos = leftHanded ? leftHoldingPosition : rightHoldingPosition;
            (leftHanded ? leftHandTarget : rightHandTarget).DOLocalMove(pos, holdTime);
            return pickuppable.transform.DOLocalMove(pos, holdTime).WaitForCompletion();
        }

        public YieldInstruction Drop(Vector3 position, Vector3 rotation)
        {
            var heldItem = heldItems[0];
            if (!heldItem) throw new Exception("Drop called with no held item.");
            
            heldItems.Remove(heldItem);

            var sequence = DOTween.Sequence();

            var tempHeight = position.y + heldItem.StandingDistance;
            
            position.y += heldItem.StandingDistance + dropHeight;
            
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
