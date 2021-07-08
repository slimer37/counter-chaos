using Core;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;

namespace Interactables.Holding
{
    public class ItemHolder : MonoBehaviour
    {
        [SerializeField] Vector3 holdingPosition;
        [SerializeField] float pickupTime;
        [SerializeField, Layer] int heldObjectLayer;
        
        [Header("Tossing")]
        [SerializeField] new Transform camera;
        [SerializeField] Vector3 tossFromPosition;
        [SerializeField] float timeToTossPosition;
        [SerializeField] float minTossForce;
        [SerializeField] float maxTossForce;
        [SerializeField] float maxForceHoldTime;

        Pickuppable heldItem;
        int tempLayer;
        
        bool isHoldingToss;
        float holdTime;

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(transform.TransformPoint(holdingPosition), Vector3.one * 0.5f);
        }

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
            
            // Button press and release conditions
            if (value.isPressed)
            {
                holdTime = 0;
                isHoldingToss = true;
                heldItem.transform.parent = camera;
                
                // Finish pickup animation in case it's still running
                heldItem.transform.DOComplete();
                
                heldItem.transform.DOLocalMove(tossFromPosition, timeToTossPosition);
            }
            else
                Drop(true);
        }

        void Update()
        {
            if (isHoldingToss)
                holdTime += Time.deltaTime;
        }

        void Drop(bool toss)
        {
            if (!heldItem) return;
            
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
