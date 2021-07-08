using Core;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Interactables.Holding
{
    public class ItemHolder : MonoBehaviour
    {
        [SerializeField] Vector3 holdingPosition;
        [SerializeField] float pickupTime;
        [SerializeField, Layer] int heldObjectLayer;

        [Header("Drop Obstacle Detection")]
        [SerializeField] Vector3 checkOrigin;
        [SerializeField] Vector3 checkExtents;
        [SerializeField] LayerMask obstacleMask;
        
        [Header("Tossing")]
        [SerializeField] new Transform camera;
        [SerializeField] Vector3 tossFromPosition;
        [SerializeField] float timeToTossPosition;
        [SerializeField] float minTossForce;
        [SerializeField] float maxTossForce;
        [SerializeField] float maxForceHoldTime;
        [SerializeField] Image holdIndicator;

        Pickuppable heldItem;
        int tempLayer;
        
        bool isHoldingToss;
        float holdTime;

        readonly Collider[] obstacleResults = new Collider[3];

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(transform.TransformPoint(holdingPosition), Vector3.one * 0.5f);
            Gizmos.color = new Color(1, 0, 0, 0.3f);
            Gizmos.DrawCube(transform.TransformPoint(checkOrigin), checkExtents * 2);
        }

        bool CheckForObstacles() => Physics.OverlapBoxNonAlloc(transform.TransformPoint(checkOrigin), checkExtents,
            obstacleResults, transform.rotation, obstacleMask) > 0;

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
        void OnToss(InputValue value)
        {
            if (!heldItem) return;

            holdIndicator.enabled = value.isPressed;
            isHoldingToss = value.isPressed;
            
            // Button press and release conditions
            if (value.isPressed)
            {
                holdTime = 0;
                heldItem.transform.parent = camera;
                
                // Finish pickup animation in case it's still running
                heldItem.transform.DOComplete();
                
                heldItem.transform.DOLocalMove(tossFromPosition, timeToTossPosition);
            }
            else
            {
                // Return the item to the holding position if an obstacle is detected.
                if (CheckForObstacles())
                    heldItem.transform.DOLocalMove(holdingPosition, timeToTossPosition);
                else
                    Drop(true);
            }
        }

        void Update()
        {
            if (isHoldingToss)
            {
                holdTime += Time.deltaTime;
                holdIndicator.fillAmount = Mathf.Clamp01(holdTime / maxForceHoldTime);
            }
        }

        void Drop(bool toss)
        {
            if (!heldItem || CheckForObstacles()) return;
            
            // Finish tweens if still in progress.
            heldItem.transform.DOComplete();
            
            heldItem.gameObject.layer = tempLayer;
            
            if (toss)
            {
                var tossForce = Mathf.Lerp(minTossForce, maxTossForce, holdTime / maxForceHoldTime);
                heldItem.Toss(camera.forward, tossForce);
            }
            else
                heldItem.Drop();

            heldItem = null;
        }
    }
}
