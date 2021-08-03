using DG.Tweening;
using Interactables.Base;
using UnityEngine;

namespace Interactables.Inspection
{
    public class Inspectable : MonoBehaviour, IInteractHandler, IStopInteractHandler
    {
        [SerializeField] Hoverable hoverable;
        [SerializeField] float distanceFromCamera;
        [SerializeField] float minInteractDist;
        [SerializeField] float maxDistFromOriginalPos;
        [SerializeField] Vector3 offsetRotation;
        [SerializeField] Vector3 forwardDirection = Vector3.forward;
        [SerializeField, Min(0)] float angleAllowance = 180;
        [SerializeField] float animTime;
        
        static Transform playerCamera;
        static bool itemBeingInspected;

        Vector3 originalPosition;
        Quaternion originalRotation;
        Vector3 angleCheckVector;

        PlayerController tempController;

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, maxDistFromOriginalPos);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, minInteractDist);
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, transform.TransformDirection(forwardDirection));
        }

        void Awake()
        {
            // Can hover if no item is being inspected.
            hoverable.OnAttemptHover += _ => CanInteract();
            if (!playerCamera)
                playerCamera = Camera.main.transform;

            originalPosition = transform.position;
            originalRotation = transform.rotation;
            angleCheckVector = transform.TransformDirection(-forwardDirection);
        }

        bool CanInteract() =>
            !itemBeingInspected
            && (playerCamera.position - transform.position).sqrMagnitude > minInteractDist * minInteractDist
            && (transform.position - originalPosition).sqrMagnitude < maxDistFromOriginalPos * maxDistFromOriginalPos
            && Vector3.Dot(playerCamera.forward, angleCheckVector) >= (180 - angleAllowance) / 180;

        public void OnInteract(Transform sender)
        {
            if (!CanInteract()) return;

            (tempController = sender.GetComponent<PlayerController>()).Suspend();
            itemBeingInspected = true;
            hoverable.enabled = false;

            transform.DOKill();

            transform.DOMove(playerCamera.position + playerCamera.forward * distanceFromCamera, animTime);
            var rot = Quaternion.LookRotation(-playerCamera.forward, playerCamera.up).eulerAngles + offsetRotation;
            transform.DORotateQuaternion(Quaternion.Euler(rot), animTime);
        }

        public void OnStopInteract(Transform sender)
        {
            if (!tempController) return;
            tempController.Suspend(false);
            itemBeingInspected = false;
            hoverable.enabled = true;
            
            transform.DOMove(originalPosition, animTime);
            transform.DORotateQuaternion(originalRotation, animTime);
        }
    }
}
