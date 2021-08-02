using DG.Tweening;
using Interactables.Base;
using UnityEngine;

namespace Interactables.Inspection
{
    public class Inspectable : MonoBehaviour, IInteractHandler, IStopInteractHandler
    {
        [SerializeField] Hoverable hoverable;
        [SerializeField] float distanceFromCamera;
        [SerializeField] Vector3 offsetRotation;
        [SerializeField] float animTime;
        
        static Transform playerCamera;
        static bool itemBeingInspected;

        Vector3 originalPosition;
        Quaternion originalRotation;

        PlayerController tempController;
    
        void Awake()
        {
            // Can hover if no item is being inspected.
            hoverable.OnAttemptHover += _ => CanInteract();
            if (!playerCamera)
                playerCamera = Camera.main.transform;

            originalPosition = transform.position;
            originalRotation = transform.rotation;
        }

        bool CanInteract() =>
            !itemBeingInspected
            && (playerCamera.position - transform.position).sqrMagnitude > distanceFromCamera * distanceFromCamera;
        
        public void OnInteract(Transform sender)
        {
            if (!CanInteract()) return;

            (tempController = sender.GetComponent<PlayerController>()).Suspend();
            itemBeingInspected = true;
            hoverable.enabled = false;

            transform.DOMove(playerCamera.position + playerCamera.forward * distanceFromCamera, animTime);
            var rot = Quaternion.LookRotation(-playerCamera.forward, playerCamera.up).eulerAngles + offsetRotation;
            transform.DORotateQuaternion(Quaternion.Euler(rot), animTime);
        }

        public void OnStopInteract(Transform sender)
        {
            tempController.Suspend(false);
            itemBeingInspected = false;
            hoverable.enabled = true;
            transform.DOMove(originalPosition, animTime);
            transform.DORotateQuaternion(originalRotation, animTime);
        }
    }
}
