using Core;
using DG.Tweening;
using Interactables.Base;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Doors
{
    public class Door : MonoBehaviour, IInteractable
    {
        [Header("Pulling")]
        [SerializeField] bool invert;
        [SerializeField] bool showPullByDefault;
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

        Hoverable hoverable;
        Controls controls;

        public InteractionIcon Icon { get; protected set; }

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
            controls.Gameplay.Interact.canceled += OnRelease;
            
            var rend = GetComponentInChildren<Renderer>();
            if (!rend) Debug.LogWarning("No renderer found to calculate door center.", this);
            center = transform.InverseTransformPoint(rend.bounds.center);
        }

        void OnDestroy() => controls.Dispose();

        public bool CanInteract(Transform sender)
        {
            UpdateIcon();
            return true;
        }

        public void OnInteract(Transform sender)
        {
            isInteracting = true;
            if (resetTween.IsActive())
                resetTween.Kill();
            controls.Enable();
        }

        void UpdateIcon()
        {
            var yRot = transform.localEulerAngles.y;
            if (yRot > 180) yRot -= 360;
            var facingFront = Vector3.Dot(Player.Camera.transform.forward, transform.forward) < 0;
            
            bool push;
            if (showPullByDefault)
                push = facingFront != invert ? yRot >= rotationMax : yRot <= rotationMin;
            else
                push = facingFront != invert ? yRot > rotationMin : yRot < rotationMax;
            
            Icon = push ? InteractionIcon.Push : InteractionIcon.Pull;
        }

        void Update()
        {
            if (isInteracting)
            {
                var a = Player.Camera.transform.TransformPoint(Vector3.forward * PullDistance);
                var b = Quaternion.Inverse(transform.rotation) * (a - transform.TransformPoint(center));
                delta = b.z * RotationSpeed * (invert ? -1 : 1);
            }
            else if (delta == 0)
                return;
            
            var rot = transform.localEulerAngles;
            if (rot.y > 180) rot.y -= 360;
            rot.y = Mathf.Clamp(rot.y + delta * Time.deltaTime, rotationMin, rotationMax);
            transform.localEulerAngles = rot;
            
            UpdateIcon();
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
