using Core;
using DG.Tweening;
using Interactables.Base;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Interactables
{
    public class Door : MonoBehaviour, IInteractHandler
    {
        [Header("Pulling")]
        [SerializeField] float pullDistance = 1;
        [SerializeField] float rotationSpeed = 50;
        [Space]
        [SerializeField, Range(-180, 0)] float rotationMin = -180;
        [SerializeField, Range(0, 180)] float rotationMax = 180;
        [SerializeField] float stopTime = 0.5f;

        Tween resetTween;
        Transform playerCamera;

        Vector3 center;
        float delta;
        bool isInteracting;

        Controls controls;

        void OnValidate()
        {
            for (var i = 0; i < 3; i++)
                if (transform.lossyScale[i] < 0)
                    Debug.LogWarning("Door does not rotate correctly with negative scales.", gameObject);
        }

        void Awake()
        {
            controls = new Controls();
            controls.Gameplay.Interact.canceled += OnRelease;
            
            playerCamera = Camera.main.transform;
        }

        void OnDestroy() => controls.Dispose();

        public void OnInteract(Transform sender)
        {
            Physics.Raycast(new Ray(playerCamera.position, playerCamera.forward), out var hit);
            center = transform.InverseTransformPoint(hit.point);
            isInteracting = true;
            if (resetTween.IsActive())
                resetTween.Kill();
            controls.Enable();
        }

        void Update()
        {
            if (isInteracting)
            {
                var a = playerCamera.TransformPoint(Vector3.forward * pullDistance);
                var b = Quaternion.Inverse(transform.rotation) * (a - transform.TransformPoint(center));
                delta = b.z * rotationSpeed;
            }
            var rot = transform.localEulerAngles;
            if (rot.y > 180) rot.y -= 360;
            rot.y = Mathf.Clamp(rot.y + delta * Time.deltaTime, rotationMin, rotationMax);
            transform.localEulerAngles = rot;
        }

        public void OnRelease(InputAction.CallbackContext ctx)
        {
            if (!isInteracting) return;
            isInteracting = false;
            controls.Disable();
            resetTween = DOTween.To(() => delta, a => delta = a, 0, stopTime);
        }
    }
}
