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
        [SerializeField, Layer] int droppingObjectLayer;

        [Header("Item Manipulation")]
        [SerializeField] float rotationSpeed;
        [SerializeField] float flatSurfaceTolerance;
        [SerializeField] float correctionAmount = 0.01f;
        [SerializeField] int correctionLimit = 10;
        
        [Header("Obstacle Detection")]
        [SerializeField] float lineOfSightMaxDist = 5;
        [SerializeField] LayerMask tossObstacleMask;
        [SerializeField] LayerMask cameraBlockRaycastMask;
        [SerializeField] LayerMask dropObstacleMask;
        
        [Header("Dropping")]
        [SerializeField] Vector3 defaultDropPosition;
        [SerializeField] float dropReach;
        [SerializeField] float extraDropHeight;
        [SerializeField] LayerMask dropSurfaceMask;
        [SerializeField] LayerMask groundLayerMask;
        [SerializeField] Ghost ghost;

        [Header("Tossing")]
        [SerializeField] Vector3 tossFromPosition;
        [SerializeField] float minTossForce;
        [SerializeField] float maxTossForce;
        [SerializeField] float maxForceHoldTime;
        [SerializeField] Image holdIndicator;

        public bool IsHoldingItem => heldItem;
        public Pickuppable HeldItem => heldItem;

        PlayerController controller;
        
        Pickuppable heldItem;
        Vector3 holdingPosition;
        Quaternion holdingRotation;
        readonly Quaternion dropOrThrowRotation = Quaternion.LookRotation(Vector3.back);

        float rotationDelta;
        int tempLayer;
        
        bool isHoldingToss;
        bool isHoldingDrop;
        bool isRotating;
        float holdTime;

        bool inDefaultDropPos;
        bool onFlatSurface;
        
        Inventory inv;

        readonly Collider[] obstacleResults = new Collider[1];

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(transform.TransformPoint(defaultHoldingPosition), Vector3.one * 0.25f);
        }

        void Awake()
        {
            controller = GetComponent<PlayerController>();
        }

        internal Pickuppable StopHolding()
        {
            if (!heldItem) throw new NullReferenceException($"{nameof(StopHolding)} called without a held item.");
            
            var temp = heldItem;
            Drop(false, false);
            controller.EnableLook(true);
            holdIndicator.enabled = false;
            return temp;
        }
        
        internal void Hold(Pickuppable pickuppable, Inventory inv)
        {
            if (heldItem) throw new InvalidOperationException("Cannot give item while player is holding an item.");
            this.inv = inv;

            holdingPosition = pickuppable.OverridePosition ?? defaultHoldingPosition;
            holdingRotation = Quaternion.Euler(pickuppable.OverrideRotation ?? defaultHoldingRotation);
            pickuppable.Setup(transform);
            heldItem = pickuppable;
            
            if (pickuppable.Info.canBeDropped)
                ghost.SetMesh(pickuppable.transform);
            
            tempLayer = heldItem.gameObject.layer;
            
            SetHeldObjectLayers(heldObjectLayer);
            MoveAndRotateHeldItem(holdingPosition, holdingRotation, pickupTime);
        }

        void OnRotate(InputValue value)
        {
            isRotating = value.isPressed;
            if (!isHoldingDrop) return;
            controller.EnableLook(!isRotating);
        }

        void MoveAndRotateHeldItem(Vector3 position, Quaternion rotation, float time)
        {
            // Finish tweens if still in progress.
            heldItem.transform.DOKill();
            heldItem.transform.DOLocalMove(position, time);
            heldItem.transform.DOLocalRotateQuaternion(rotation, time);
        }

        void ReturnItemToHolding()
        {
            SetHeldObjectLayers(heldObjectLayer);
            MoveAndRotateHeldItem(holdingPosition, holdingRotation, timeBetweenHoldPositions);
            ghost.Hide();
        }

        void OnDrop(InputValue value)
        {
            if (!heldItem || !heldItem.Info.canBeDropped || isHoldingToss) return;
            
            // On hold
            if (value.isPressed)
            {
                isHoldingDrop = true;
                heldItem.transform.localRotation = dropOrThrowRotation;
                heldItem.transform.DOKill();
            }
            // On release
            else if (isHoldingDrop)
            {
                isHoldingDrop = false;

                controller.EnableLook(true);

                if (!heldItem.IsIntersecting(dropObstacleMask, obstacleResults)
                    && (!heldItem.Info.groundPlacementOnly || onFlatSurface)
                    && (!inDefaultDropPos || !IsLineOfSightBlocked()))
                    Drop(false);
                else
                    ReturnItemToHolding();
            }
        }
        
        void SetHeldObjectLayers(int layer)
        {
            if (heldItem.gameObject.layer == layer) return;
            heldItem.gameObject.layer = layer;
            foreach (Transform child in heldItem.transform)
                child.gameObject.layer = layer;
        }
        
        void OnToss(InputValue value)
        {
            if (!heldItem || !heldItem.Info.canBeThrown || !heldItem.Info.canBeDropped || isHoldingDrop) return;

            holdIndicator.enabled = value.isPressed;
            
            // On hold
            if (value.isPressed)
            {
                holdTime = 0;
                isHoldingToss = true;
                heldItem.transform.parent = camera.transform;
                MoveAndRotateHeldItem(tossFromPosition, dropOrThrowRotation, timeBetweenHoldPositions);
            }
            // On release
            else if (isHoldingToss)
            {
                isHoldingToss = false;
                
                // Return the item to the holding position if an obstacle is detected.
                if (CheckForTossObstacles())
                {
                    heldItem.transform.parent = transform;
                    ReturnItemToHolding();
                }
                else
                    Drop(true);
            }

            bool CheckForTossObstacles() =>
                heldItem.IsIntersecting(tossObstacleMask, obstacleResults)
                || IsLineOfSightBlocked();
        }
        
        bool IsLineOfSightBlocked()
        {
            var camPos = camera.transform.position;
            var camItemRay = new Ray(camPos, heldItem.transform.position - camPos);
            var temp = Physics.Raycast(camItemRay, out var hit, lineOfSightMaxDist, cameraBlockRaycastMask)
                && hit.transform != heldItem.transform;
            return temp;
        }

        Ray GetCameraRay() => camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        void OnMoveMouse(InputValue val)
        {
            if (!isHoldingDrop || !isRotating) return;
            rotationDelta = val.Get<Vector2>().x * rotationSpeed;
        }

        void Update()
        {
            if (!heldItem) return;
            
            if (isHoldingToss)
            {
                holdTime += Time.deltaTime;
                holdIndicator.fillAmount = Mathf.Clamp01(holdTime / maxForceHoldTime);
            }

            if (isHoldingDrop)
            {
                var itemTransform = heldItem.transform;
                
                if (isRotating)
                    itemTransform.localEulerAngles += rotationDelta * Time.deltaTime * Vector3.up;

                var onFreeSpot = false;
                var rayHit = Physics.Raycast(GetCameraRay(), out var hit, dropReach, dropSurfaceMask);

                if (rayHit)
                {
                    var distanceOffSurface = extraDropHeight;

                    // Only use bound diagonal if the surface is not horizontal (e.g. the ground but not the ceiling).
                    var angle = Vector3.Angle(hit.normal, Vector3.up);
                    onFlatSurface = angle < flatSurfaceTolerance;
                    distanceOffSurface += onFlatSurface ? heldItem.VerticalExtent : heldItem.BoundHalfDiagonal;

                    itemTransform.position = hit.point + hit.normal * distanceOffSurface;

                    if (heldItem.IsIntersecting(dropObstacleMask, obstacleResults))
                    {
                        for (var i = 0; i < correctionLimit; i++)
                        {
                            itemTransform.position += Vector3.up * correctionAmount;
                            if (AttemptOffset(-transform.forward)
                                || AttemptOffset(-hit.normal)
                                || AttemptOffset(-transform.right)
                                || AttemptOffset(transform.right))
                            {
                                onFreeSpot = true;
                                break;
                            }
                        }
                    }
                    else
                        onFreeSpot = true;
                    
                    // Only try moving item forward if surface is a wall (within 10 degrees of freedom).
                    if (Math.Abs(angle - 90) < 10)
                        AttemptOffset(-hit.normal, true);

                    bool AttemptOffset(Vector3 direction, bool useFarthest = false)
                    {
                        direction *= correctionAmount;
                        var temp = itemTransform.position;
                        var limits = Mathf.Min(correctionLimit, heldItem.BoundHalfDiagonal / correctionAmount);
                        for (var i = 0; i <= limits; i++)
                        {
                            itemTransform.position += direction;
                            if (heldItem.IsIntersecting(dropObstacleMask, obstacleResults))
                            {
                                if (useFarthest)
                                {
                                    itemTransform.position -= direction;
                                    break;
                                }
                            }
                            else if (!useFarthest) return true;
                        }
                        if (!useFarthest) itemTransform.position = temp;
                        return false;
                    }
                }
                else
                    onFlatSurface = false;
                
                if (onFlatSurface)
                    onFlatSurface &= onFreeSpot && hit.transform.gameObject.IsInLayerMask(groundLayerMask);
                
                if (onFreeSpot && (!heldItem.Info.groundPlacementOnly || onFlatSurface))
                {
                    ghost.Hide();
                    inDefaultDropPos = false;
                }
                else
                {
                    if (rayHit) ghost.ShowAt(itemTransform.position, itemTransform.rotation);
                    else ghost.Hide();
                    itemTransform.position = transform.TransformPoint(defaultDropPosition);
                    inDefaultDropPos = true;
                }
                
                SetHeldObjectLayers(onFreeSpot ? droppingObjectLayer : heldObjectLayer);
            }
        }

        void Drop(bool addForce, bool clearSlot = true)
        {
            if (!heldItem) throw new NullReferenceException("Drop called without a held item.");
            
            // Finish tweens if still in progress.
            heldItem.transform.DOKill();
            
            SetHeldObjectLayers(tempLayer);
            
            if (addForce)
            {
                var tossForce = Mathf.Lerp(minTossForce, maxTossForce, holdTime / maxForceHoldTime);
                heldItem.Toss(camera.transform.forward, tossForce);
            }
            else
                heldItem.Drop();
            
            isHoldingToss = false;
            isHoldingDrop = false;

            heldItem = null;
            ghost.Hide();

            if (clearSlot) inv.ClearActiveSlot();
        }
    }
}
