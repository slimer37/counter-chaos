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
        
        [Header("Magnetic Close")]
        [SerializeField] float closeMagnetism = 5;
        [SerializeField] float magnetThreshold;

        [Header("Events")]
        [SerializeField, Tooltip("How close the door should be to its closed rotation to be considered closed.")]
        float closedThreshold = 1;
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
            var closedRotation = invert ? rotationMax : rotationMin;
            var rot = transform.localEulerAngles;
            
            // How far open the door is in degrees.
            var openness = Math.Abs(rot.y - closedRotation);
            
            var openState = openness > closedThreshold;
            var magnetState = openness < magnetThreshold;
            
            var magneticFactor = (magnetThreshold - openness) * closeMagnetism * (invert ? 1 : -1);
            
            if (isInteracting)
            {
                var pullPoint = Player.Camera.transform.TransformPoint(Vector3.forward * PullDistance);
                var forward = transform.InverseTransformPoint(pullPoint).z;

                if ((pullPoint - transform.position).sqrMagnitude > Range * Range)
                {
                    OnRelease();
                    return;
                }
                
                delta = forward * RotationSpeed * (invert ? -1 : 1);
                
                if (magnetState)
                    delta += magneticFactor;
            }
            else if (magnetState)
                delta += magneticFactor * Time.deltaTime;
            else if (delta != 0)
                delta = Mathf.MoveTowards(delta, 0, requiredDeceleration * Time.deltaTime);
            
            if (rot.y > 180) rot.y -= 360;
            rot.y = Mathf.Clamp(rot.y + delta * Time.deltaTime, rotationMin, rotationMax);
            transform.localEulerAngles = rot;

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
            if (!isInteracting) return;
            
            // Decelerate such that delta approaches 0 in stopTime seconds.
            requiredDeceleration = Mathf.Abs(delta / stopTime);
            channel.Deactivate();
            isInteracting = false;
            controls.Disable();
        }
    }
}
