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
        [SerializeField] Vector3 defaultHoldingPosition;
        [SerializeField] Vector3 defaultHoldingRotation;
        [SerializeField] float pickupTime;
        [SerializeField] float timeBetweenHoldPositions;
        [SerializeField, Layer] int heldObjectLayer;

        [Header("Item Manipulation")]
        [SerializeField] float rotationSpeed;
        [SerializeField] float flatSurfaceTolerance;
        
        [Header("Obstacle Detection")]
        [SerializeField] LayerMask tossObstacleMask;
        [SerializeField] LayerMask cameraBlockRaycastMask;
        [SerializeField] LayerMask dropObstacleMask;
        
        [Header("Dropping")]
        [SerializeField] Vector3 defaultDropPosition;
        [SerializeField] float dropReach;
        [SerializeField] float extraDropHeight;
        [SerializeField] LayerMask dropSurfaceMask;

        [Header("Tossing")]
        [SerializeField] Vector3 tossFromPosition;
        [SerializeField] float minTossForce;
        [SerializeField] float maxTossForce;
        [SerializeField] float maxForceHoldTime;
        [SerializeField] Image holdIndicator;

        public bool IsHoldingItem => heldItem;
        public Pickuppable HeldItem => heldItem;
        
        Pickuppable heldItem;
        Vector3 holdingPosition;
        Quaternion holdingRotation;
        int tempLayer;
        
        bool isHoldingToss;
        bool isHoldingDrop;
        bool isRotating;
        float holdTime;

        readonly Collider[] obstacleResults = new Collider[1];

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(transform.TransformPoint(defaultHoldingPosition), Vector3.one * 0.25f);
        }

        public Pickuppable TakeFrom()
        {
            if (!heldItem) throw new NullReferenceException("TakeFrom called without a held item.");
            
            var temp = heldItem;
            Drop(false);
            return temp;
        }
        
        public void Give(Pickuppable pickuppable)
        {
            if (heldItem) throw new InvalidOperationException("Cannot give item while player is holding an item.");

            holdingPosition = pickuppable.OverridePosition ?? defaultHoldingPosition;
            holdingRotation = Quaternion.Euler(pickuppable.OverrideRotation ?? defaultHoldingRotation);
            pickuppable.Setup(transform);
            heldItem = pickuppable;
            
            var go = heldItem.gameObject;
            tempLayer = go.layer;
            
            go.layer = heldObjectLayer;
            foreach (Transform child in heldItem.transform)
                child.gameObject.layer = heldObjectLayer;
            
            MoveAndRotateHeldItem(holdingPosition, holdingRotation, pickupTime);
        }

        void OnRotate(InputValue value) => isRotating = value.isPressed;

        void MoveAndRotateHeldItem(Vector3 position, Quaternion rotation, float time)
        {
            // Finish tweens if still in progress.
            heldItem.transform.DOKill();
            heldItem.transform.DOLocalMove(position, time);
            heldItem.transform.DOLocalRotateQuaternion(rotation, time);
        }

        void OnDrop(InputValue value)
        {
            if (!heldItem || isHoldingToss) return;
            
            // On hold
            if (value.isPressed)
            {
                isHoldingDrop = true;
                heldItem.transform.localRotation = Quaternion.identity;
                heldItem.transform.DOKill();
            }
            // On release
            else if (isHoldingDrop)
            {
                isHoldingDrop = false;
                
                if (heldItem.IsIntersecting(dropObstacleMask, obstacleResults))
                    MoveAndRotateHeldItem(holdingPosition, holdingRotation, timeBetweenHoldPositions);
                else
                    Drop(false);
            }
        }
        
        void OnToss(InputValue value)
        {
            if (!heldItem || !heldItem.Throwable || isHoldingDrop) return;

            holdIndicator.enabled = value.isPressed;
            
            // On hold
            if (value.isPressed)
            {
                holdTime = 0;
                isHoldingToss = true;
                ParentAndPosition(camera.transform, tossFromPosition, Quaternion.identity);
            }
            // On release
            else if (isHoldingToss)
            {
                isHoldingToss = false;
                
                // Return the item to the holding position if an obstacle is detected.
                if (CheckForTossObstacles())
                    ParentAndPosition(transform, holdingPosition, holdingRotation);
                else
                    Drop(true);
            }
            
            void ParentAndPosition(Transform newParent, Vector3 position, Quaternion rotation)
            {
                heldItem.transform.parent = newParent;
                MoveAndRotateHeldItem(position, rotation, timeBetweenHoldPositions);
            }

            bool CheckForTossObstacles() =>
                heldItem.IsIntersecting(tossObstacleMask, obstacleResults)
                || Physics.Raycast(GetCameraRay(), out var hit, tossFromPosition.magnitude, cameraBlockRaycastMask)
                && hit.transform != heldItem.transform;
        }

        Ray GetCameraRay() => camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

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
                
                if (isRotating)
                    itemTransform.localEulerAngles += rotationSpeed * Time.deltaTime * Vector3.up;
                
                if (Physics.Raycast(GetCameraRay(), out var hit, dropReach, dropSurfaceMask))
                {
                    var distanceOffSurface = extraDropHeight;
                    
                    // Only use bound diagonal if the surface is not horizontal (e.g. the ground but not the ceiling).
                    var angle = Vector3.Angle(hit.normal, Vector3.up);
                    distanceOffSurface += angle < flatSurfaceTolerance
                        ? heldItem.VerticalExtent : heldItem.BoundHalfDiagonal;
                    
                    itemTransform.position = hit.point + hit.normal * distanceOffSurface;
                    
                    if (heldItem.IsIntersecting(dropObstacleMask, obstacleResults))
                        SetDefaultDropPosition();
                }
                else
                    SetDefaultDropPosition();
                
                void SetDefaultDropPosition() => itemTransform.position = transform.TransformPoint(defaultDropPosition);
            }
        }

        void Drop(bool addForce)
        {
            if (!heldItem) throw new NullReferenceException("Drop called without a held item.");
            
            // Finish tweens if still in progress.
            heldItem.transform.DOKill();
            
            heldItem.gameObject.layer = tempLayer;
            
            foreach (Transform child in heldItem.transform)
                child.gameObject.layer = tempLayer;
            
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
