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
        [SerializeField] bool invert;
        [Space]
        [SerializeField, Range(-180, 0)] float rotationMin = -180;
        [SerializeField, Range(0, 180)] float rotationMax = 180;
        [SerializeField] float stopTime = 0.5f;
        
        const int RotationSpeed = 200;
        const int PullDistance = 2;

        Tween resetTween;

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
            var rend = GetComponentInChildren<Renderer>();
            if (!rend) Debug.LogWarning("No renderer found to calculate door center.", this);
            center = transform.InverseTransformPoint(rend.bounds.center);
        }

        void OnDestroy() => controls.Dispose();

        public void OnInteract(Transform sender)
        {
            isInteracting = true;
            if (resetTween.IsActive())
                resetTween.Kill();
            controls.Enable();
        }

        void Update()
        {
            if (isInteracting)
            {
                var a = Player.Camera.transform.TransformPoint(Vector3.forward * PullDistance);
                var b = Quaternion.Inverse(transform.rotation) * (a - transform.TransformPoint(center));
                delta = b.z * RotationSpeed * (invert ? -1 : 1);
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
