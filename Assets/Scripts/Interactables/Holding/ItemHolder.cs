using System;
using Core;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Interactables.Holding
{
    public class ItemHolder : MonoBehaviour
    {
        [SerializeField] new Camera camera;
        [SerializeField] Vector3 holdingPosition;
        [SerializeField] float pickupTime;
        [SerializeField] float timeBetweenHoldPositions;
        [SerializeField, Layer] int heldObjectLayer;

        [Header("Item Manipulation")]
        [SerializeField] float rotationSpeed;
        
        [Header("Drop Obstacle Detection")]
        [SerializeField] Vector3 tossCheckOrigin;
        [SerializeField] Vector3 tossCheckExtents;
        [SerializeField] LayerMask tossObstacleMask;
        [SerializeField] LayerMask dropObstacleMask;
        [SerializeField] float dropCheckIncrease;
        
        [Header("Dropping")]
        [SerializeField] Vector3 defaultDropPosition;
        [SerializeField] float dropReach;
        [SerializeField] float dropHeight;
        [SerializeField] LayerMask dropSurfaceMask;

        [Header("Tossing")]
        [SerializeField] Vector3 tossFromPosition;
        [SerializeField] float minTossForce;
        [SerializeField] float maxTossForce;
        [SerializeField] float maxForceHoldTime;
        [SerializeField] Image holdIndicator;

        Pickuppable heldItem;
        int tempLayer;
        
        bool isHoldingToss;
        bool isHoldingDrop;
        bool isRotating;
        float holdTime;

        readonly Collider[] obstacleResults = new Collider[1];

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(transform.TransformPoint(holdingPosition), Vector3.one * 0.5f);
            
            Gizmos.color = new Color(1, 0, 0, 0.3f);
            Gizmos.DrawCube(transform.TransformPoint(tossCheckOrigin), tossCheckExtents * 2);
        }

        void OnValidate()
        {
            if (dropCheckIncrease > dropHeight)
                Debug.LogWarning($"{nameof(dropCheckIncrease)} is greater than {nameof(dropHeight)}. Intersection checks will always fail.");
        }

        void OnEnable() => Pickuppable.ItemPickedUp += OnPickup; 
        void OnDisable() => Pickuppable.ItemPickedUp -= OnPickup;

        void OnPickup(Pickuppable pickuppable)
        {
            heldItem = pickuppable;
            var go = heldItem.gameObject;
            tempLayer = go.layer;
            go.layer = heldObjectLayer;
            
            MoveAndRotateHeldItem(holdingPosition, pickupTime);
        }

        void OnRotate(InputValue value) => isRotating = value.isPressed;

        void MoveAndRotateHeldItem(Vector3 position, float time)
        {
            // Finish tweens if still in progress.
            heldItem.transform.DOComplete();
            heldItem.transform.DOLocalMove(position, time);
            heldItem.transform.DOLocalRotateQuaternion(Quaternion.identity, time);
        }

        void OnDrop(InputValue value)
        {
            if (!heldItem || isHoldingToss) return;
            
            // On hold
            if (value.isPressed)
            {
                isHoldingDrop = true;
                heldItem.transform.DOComplete();
            }
            // On release
            else if (isHoldingDrop)
            {
                isHoldingDrop = false;
                
                if (heldItem.IsIntersecting(dropObstacleMask, obstacleResults, dropCheckIncrease))
                    MoveAndRotateHeldItem(holdingPosition, timeBetweenHoldPositions);
                else
                    Drop(false);
            }
        }
        
        void OnToss(InputValue value)
        {
            if (!heldItem || isHoldingDrop) return;

            holdIndicator.enabled = value.isPressed;
            
            // On hold
            if (value.isPressed)
            {
                holdTime = 0;
                isHoldingToss = true;
                ParentAndPosition(camera.transform, tossFromPosition);
            }
            // On release
            else if (isHoldingToss)
            {
                isHoldingToss = false;
                
                // Return the item to the holding position if an obstacle is detected.
                if (CheckForTossObstacles(true))
                    ParentAndPosition(transform, holdingPosition);
                else
                    Drop(true);
            }
            
            void ParentAndPosition(Transform newParent, Vector3 position)
            {
                heldItem.transform.parent = newParent;
                MoveAndRotateHeldItem(position, timeBetweenHoldPositions);
            }
            
            bool CheckForTossObstacles(bool useTossCheckBox) => Physics.OverlapBoxNonAlloc(transform.TransformPoint(tossCheckOrigin),
                tossCheckExtents, obstacleResults, transform.rotation, tossObstacleMask) > 0;
        }

        void Update()
        {
            if (isHoldingToss)
            {
                holdTime += Time.deltaTime;
                holdIndicator.fillAmount = Mathf.Clamp01(holdTime / maxForceHoldTime);
            }

            if (isHoldingDrop)
            {
                var itemTransform = heldItem.transform;
                var itemPosition = transform.TransformPoint(defaultDropPosition);
                if (Physics.Raycast(camera.ViewportPointToRay(new Vector3(0.5f, 0.5f)), out var hit, dropReach, dropSurfaceMask))
                    itemPosition = hit.point + hit.normal * dropHeight;
                itemTransform.position = itemPosition;
                
                if (heldItem.IsIntersecting(dropObstacleMask, obstacleResults, dropCheckIncrease))
                    itemTransform.position = transform.TransformPoint(defaultDropPosition);

                if (isRotating)
                    itemTransform.localEulerAngles += rotationSpeed * Time.deltaTime * Vector3.up;
            }
        }

        void Drop(bool addForce)
        {
            if (!heldItem) throw new NullReferenceException("Drop called without a held item.");
            
            // Finish tweens if still in progress.
            heldItem.transform.DOComplete();
            
            heldItem.gameObject.layer = tempLayer;
            
            if (addForce)
            {
                var tossForce = Mathf.Lerp(minTossForce, maxTossForce, holdTime / maxForceHoldTime);
                heldItem.Toss(camera.transform.forward, tossForce);
            }
            else
                heldItem.Drop();

            heldItem = null;
        }
    }
}
