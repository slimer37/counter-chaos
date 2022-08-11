using System;
using Core;
using Interactables.Base;
using UnityEngine;
using UnityEngine.Events;

namespace Doors
{
    public class Door : MonoBehaviour, IInteractable
    {
        [Header("Pulling")]
        [SerializeField] bool invert;
        [Space]
        [SerializeField, Range(-180, 0)] float rotationMin = -180;
        [SerializeField, Range(0, 180)] float rotationMax = 180;
        [SerializeField] float stopTime = 0.5f;

        [Header("Events")]
        [SerializeField, Tooltip("How close the door should be to its closed rotation to be considered closed.")]
        float closedThreshold = 10;
        [SerializeField] UnityEvent onOpen;
        [SerializeField] UnityEvent onClose;
        [SerializeField] InteractionChannel channel;
        
        const int RotationSpeed = 200;
        const int PullDistance = 2;
        const int Range = 5;

        float delta;
        float requiredDeceleration;
        bool isInteracting;

        bool isOpen;

        Hoverable hoverable;
        Controls controls;

        public InteractionIcon Icon => InteractionIcon.Door;

        void OnValidate()
        {
            for (var i = 0; i < 3; i++)
                if (transform.lossyScale[i] < 0)
                    Debug.LogWarning("Door does not rotate correctly with negative scales.", gameObject);
        }

        void Awake()
        {
            // Release door when releasing control, not when hovering off.
            controls = new Controls();
            controls.Gameplay.Interact.canceled += _ => OnRelease();
        }

        void OnDestroy() => controls.Dispose();

        public void OnInteract(Transform sender)
        {
            isInteracting = true;
            channel.Activate();
            controls.Enable();
        }

        void Update()
        {
            if (isInteracting)
            {
                var pullPoint = Player.Camera.transform.TransformPoint(Vector3.forward * PullDistance);
                var forward = transform.InverseTransformPoint(pullPoint).z;

                if ((Player.Camera.transform.position - transform.position).sqrMagnitude > Range * Range)
                {
                    OnRelease();
                    return;
                }
                
                delta = forward * RotationSpeed * (invert ? -1 : 1);
            }
            else if (delta != 0)
                delta = Mathf.MoveTowards(delta, 0, requiredDeceleration * Time.deltaTime);
            else
                return;
            
            var rot = transform.localEulerAngles;
            if (rot.y > 180) rot.y -= 360;
            rot.y = Mathf.Clamp(rot.y + delta * Time.deltaTime, rotationMin, rotationMax);
            transform.localEulerAngles = rot;

            var openState = Math.Abs(rot.y - (invert ? rotationMax : rotationMin)) > closedThreshold;
            if (openState != isOpen)
            {
                isOpen = openState;
                if (isOpen)
                    onOpen?.Invoke();
                else
                    onClose?.Invoke();
            }
        }

        void OnRelease()
        {
            // Decelerate such that delta approaches 0 in stopTime seconds.
            requiredDeceleration = Mathf.Abs(delta / stopTime);
            channel.Deactivate();
            isInteracting = false;
            controls.Disable();
        }
    }
}
