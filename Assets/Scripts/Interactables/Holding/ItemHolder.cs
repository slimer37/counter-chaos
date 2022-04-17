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
        public bool useOldSystem;
        
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
        [SerializeField] Transform cursor3D;
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

        Vector3 droppingRotation;

        Vector2 mousePos;

        bool inDefaultDropPos;
        bool onFlatSurface;
        
        Inventory inv;

        public const string UseOldSystemPrefKey = "HoldSystem";

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(transform.TransformPoint(defaultHoldingPosition), Vector3.one * 0.25f);
        }

        void Awake()
        {
            controller = GetComponent<PlayerController>();
            useOldSystem = PlayerPrefs.GetInt(UseOldSystemPrefKey) == 1;
            cursor3D.gameObject.SetActive(false);
        }

        internal Pickuppable StopHolding()
        {
            if (!heldItem) throw new NullReferenceException($"{nameof(StopHolding)} called without a held item.");
            
            var temp = heldItem;
            Drop(false, false);
            controller.EnableLook(true);
            holdIndicator.enabled = false;
            cursor3D.gameObject.SetActive(false);
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
            if (!useOldSystem || !isHoldingDrop) return;
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
                droppingRotation = heldItem.transform.eulerAngles;
                heldItem.transform.DOKill();
            }
            // On release
            else if (isHoldingDrop)
            {
                isHoldingDrop = false;

                controller.EnableLook(true);

                if (!heldItem.IsIntersecting(dropObstacleMask)
                    && (!heldItem.Info.groundPlacementOnly || onFlatSurface)
                    && (!inDefaultDropPos || !IsLineOfSightBlocked()))
                    Drop(false);
                else
                    ReturnItemToHolding();
            }

            cursor3D.gameObject.SetActive(isHoldingDrop);
            if (useOldSystem) return;
            controller.EnableLook(!isHoldingDrop);
            Cursor.lockState = isHoldingDrop ? CursorLockMode.Confined : CursorLockMode.Locked;
            mousePos = new Vector2(Screen.width, Screen.height) / 2;
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

            bool CheckForTossObstacles() => heldItem.IsIntersecting(tossObstacleMask) || IsLineOfSightBlocked();
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
            if (!useOldSystem && isHoldingDrop && !isRotating)
            {
                mousePos += val.Get<Vector2>();
                mousePos.x = Mathf.Clamp(mousePos.x, 0, Screen.width);
                mousePos.y = Mathf.Clamp(mousePos.y, 0, Screen.height);
            }
            
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
                
                if (isRotating) droppingRotation += rotationDelta * Time.deltaTime * Vector3.up;
                
                itemTransform.eulerAngles = droppingRotation;

                var onFreeSpot = false;
                var point = useOldSystem ? GetCameraRay() : camera.ScreenPointToRay(mousePos);
                var rayHit = Physics.Raycast(point, out var hit, dropReach, dropSurfaceMask);
                var initialPoint = Vector3.zero;

                if (rayHit)
                {
                    var distanceOffSurface = extraDropHeight;

                    // Only use bound diagonal if the surface is not horizontal (e.g. the ground but not the ceiling).
                    var angle = Vector3.Angle(hit.normal, Vector3.up);
                    onFlatSurface = angle < flatSurfaceTolerance;
                    distanceOffSurface += onFlatSurface ? heldItem.StandingDistance : heldItem.BoundHalfDiagonal;

                    itemTransform.position = hit.point + hit.normal * distanceOffSurface;
                    initialPoint = itemTransform.position;
                    
                    cursor3D.gameObject.SetActive(true);
                    cursor3D.position = hit.point;

                    if (heldItem.IsIntersecting(dropObstacleMask))
                    {
                        // Micro-adjust until intersection stops.
                        if (AttemptOffset(-transform.forward)
                            || AttemptOffset(-hit.normal)
                            || AttemptOffset(-transform.right)
                            || AttemptOffset(transform.right))
                            onFreeSpot = true;
                    }
                    else
                        onFreeSpot = true;
                    
                    // Only try moving item forward if surface is a wall (within 10 degrees).
                    if (Math.Abs(angle - 90) < 10)
                        AttemptOffset(-hit.normal, true);
                }
                else
                {
                    onFlatSurface = false;
                    cursor3D.gameObject.SetActive(false);
                }
                
                if (onFlatSurface)
                    onFlatSurface &= onFreeSpot && hit.transform.gameObject.IsInLayerMask(groundLayerMask);
                
                if (onFreeSpot && (!heldItem.Info.groundPlacementOnly || onFlatSurface))
                {
                    ghost.Hide();
                    inDefaultDropPos = false;
                }
                else
                {
                    if (rayHit) ghost.ShowAt(initialPoint, itemTransform.rotation);
                    else ghost.Hide();
                    itemTransform.position = transform.TransformPoint(defaultDropPosition);
                    inDefaultDropPos = true;
                }
                
                SetHeldObjectLayers(onFreeSpot ? droppingObjectLayer : heldObjectLayer);
            }
        }
        
        // useFarthest moves the item as far as possible without intersection
        // Keeping it false moves the item the minimum amount to avoid intersection
        bool AttemptOffset(Vector3 direction, bool useFarthest = false)
        {
            var itemTransform = heldItem.transform;
            var original = itemTransform.position;
            var adjust = direction * correctionAmount;
            
            var hadSuccess = false;
            
            for (var i = 0; i <= correctionLimit; i++)
            {
                itemTransform.position += adjust;
                
                if (heldItem.IsIntersecting(dropObstacleMask))
                {
                    // If we're intersecting now but weren't before, bump it back and exit.
                    // (only possible with useFarthest, since first success exits)
                    if (hadSuccess)
                    {
                        itemTransform.position -= adjust;
                        break;
                    }
                }
                
                // If we're not using the farthest result and not intersecting, we're good.
                else if (!useFarthest) return true;
                // Otherwise, keep going and note that we didn't intersect once.
                else hadSuccess = true;
            }
            
            // Return item to original position upon failure.
            if (!hadSuccess) itemTransform.position = original;
            
            return useFarthest && hadSuccess;
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
