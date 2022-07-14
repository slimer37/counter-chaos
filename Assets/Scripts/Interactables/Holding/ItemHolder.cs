using System;
using System.Linq;
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
        [SerializeField] LayerMask tossObstacleMask;
        [SerializeField] LayerMask cameraBlockRaycastMask;
        [SerializeField] LayerMask dropObstacleMask;
        
        [Header("Dropping")]
        [SerializeField] Transform cursor3D;
        [SerializeField] float minDropReach = 0.5f;
        [SerializeField] float maxDropReach = 3f;
        [SerializeField] float reachChangeRate = 0.1f;
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
        public bool IsDroppingItem => isHoldingDrop;
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
        float dropReach;
        float adjustedMinReach;

        Vector3 droppingRotation;

        Vector2 mousePos;

        bool onFlatSurface;

        RaycastHit dropRayHit;

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

            dropReach = maxDropReach;
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
        
        internal void Hold(Pickuppable pickuppable)
        {
            if (heldItem) throw new InvalidOperationException("Cannot give item while player is holding an item.");

            holdingPosition = pickuppable.OverridePosition ?? defaultHoldingPosition;
            holdingRotation = Quaternion.Euler(pickuppable.OverrideRotation ?? defaultHoldingRotation);
            pickuppable.Setup(transform);
            heldItem = pickuppable;
            
            if (pickuppable.Info.canBeDropped)
                ghost.SetMesh(pickuppable.transform);
            
            tempLayer = heldItem.gameObject.layer;
            
            SetHeldObjectLayers(heldObjectLayer);
            MoveAndRotateHeldItem(holdingPosition, holdingRotation, pickupTime);
            
            adjustedMinReach = minDropReach + heldItem.BoundHalfDiagonal * 2;
            ValidateDropReach();
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

                if (!ghost.gameObject.activeSelf
                    && !heldItem.IsIntersecting(dropObstacleMask)
                    && (!heldItem.Info.groundPlacementOnly || onFlatSurface)
                    && !IsLineOfSightBlocked())
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
            var dir = heldItem.transform.position - camPos;
            var camItemRay = new Ray(camPos, dir);
            return Physics.Raycast(camItemRay, out var hit, dir.magnitude, cameraBlockRaycastMask)
                && hit.transform != heldItem.transform;
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

                Vector3 initialPoint;
                bool onFreeSpot;
                
                var ray = useOldSystem ? GetCameraRay() : camera.ScreenPointToRay(mousePos);
                var rayHit = Physics.Raycast(ray, out var hit, dropReach - heldItem.BoundHalfDiagonal,
                    dropSurfaceMask);
                
                dropRayHit = hit;

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

                    onFreeSpot = !heldItem.IsIntersecting(dropObstacleMask) ||
                                 AttemptOffsets(-transform.forward,
                                     -hit.normal,
                                     Vector3.up,
                                     -transform.right,
                                     transform.right);
                    
                    // Only try moving item forward if surface is a wall (within 10 degrees).
                    if (Math.Abs(angle - 90) < 10)
                        AttemptOffset(-hit.normal, true);
                }
                else
                {
                    var cursorPoint = ray.GetPoint(dropReach - heldItem.BoundHalfDiagonal);
                    itemTransform.position = cursorPoint + Vector3.up * heldItem.StandingDistance;
                    initialPoint = itemTransform.position;
                    
                    onFlatSurface = false;
                    cursor3D.gameObject.SetActive(true);
                    cursor3D.position = cursorPoint;

                    onFreeSpot = !heldItem.IsIntersecting(dropObstacleMask) ||
                                 AttemptOffsets(Vector3.up,
                                     -transform.forward,
                                     -transform.right,
                                     transform.right);
                }
                
                if (onFlatSurface)
                    onFlatSurface &= onFreeSpot && hit.transform.gameObject.IsInLayerMask(groundLayerMask);
                
                if (onFreeSpot && (!heldItem.Info.groundPlacementOnly || onFlatSurface))
                    ghost.Hide();
                else
                {
                    ghost.ShowAt(initialPoint, itemTransform.rotation);
                    itemTransform.localPosition = holdingPosition;
                    itemTransform.localRotation = holdingRotation;
                }
                
                SetHeldObjectLayers(droppingObjectLayer);
            }
        }

        void OnScroll(InputValue value)
        {
            var scroll = value.Get<float>();
            if (!IsDroppingItem || scroll == 0) return;
            
            // If the drop ray has hit, use current item distance when decreasing reach
            if (scroll < 0 && dropRayHit.distance != 0)
                dropReach = dropRayHit.distance;
            
            var delta = scroll > 0 ? 1 : -1;
            dropReach += delta * reachChangeRate;
            ValidateDropReach();
        }

        void ValidateDropReach() => dropReach = Mathf.Clamp(dropReach, adjustedMinReach, maxDropReach);

        // Provides alternative to large if statement using ORs to chain AttemptOffset calls
        bool AttemptOffsets(params Vector3[] directions) =>
            directions.Any(dir => AttemptOffset(dir));

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
                
                if (heldItem.IsIntersecting(dropObstacleMask) ||
                    Physics.Raycast(original, direction, correctionAmount * (i + 1), dropObstacleMask))
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

            if (clearSlot) Inventory.Main.ClearActiveSlot();
        }
    }
}
