using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerController
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] CharacterController characterController;
        
        [Header("Movement")]
        [SerializeField] float walkSpeed;
        [SerializeField] float sprintSpeed;
        [SerializeField] float transitionTime;
        [SerializeField] float sprintFov;
        [SerializeField] Ease transitionEase;
        
        [Header("Looking")]
        [SerializeField] Camera cam;
        [SerializeField] Transform body;
        [SerializeField] float sensitivity;
        [SerializeField] float rotLimit;

        Sequence sprintSpeedTransition;
        float currentSpeed;
        
        Vector2 mouseDelta;
        Vector3 camRot;
        Vector3 moveVector;

        void OnMove(InputValue val)
        {
            var moveInput = val.Get<Vector2>();
            moveVector = new Vector3(moveInput.x, moveVector.y, moveInput.y);
        }

        void OnSprint(InputValue val)
        {
            if (val.isPressed)
                sprintSpeedTransition.PlayForward();
            else
                sprintSpeedTransition.PlayBackwards();
        }
        
        void OnMoveMouse(InputValue val) => mouseDelta = sensitivity * val.Get<Vector2>();

        void Awake()
        {
            camRot = cam.transform.localEulerAngles;
            currentSpeed = walkSpeed;
            
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            sprintSpeedTransition = DOTween.Sequence();
            sprintSpeedTransition.Append(DOTween.To(() => currentSpeed,
                newSpeed => currentSpeed = newSpeed, sprintSpeed, transitionTime));
            sprintSpeedTransition.Join(cam.DOFieldOfView(sprintFov, transitionTime));
            sprintSpeedTransition.SetEase(transitionEase).SetAutoKill(false).Pause();
        }

        void OnDestroy() => sprintSpeedTransition.Kill();

        void Update()
        {
            if (!characterController.isGrounded)
                moveVector.y -= 9.81f * Time.deltaTime;
            
            characterController.Move(currentSpeed * Time.deltaTime * body.TransformDirection(moveVector));

            camRot.x = Mathf.Clamp(camRot.x - mouseDelta.y * Time.deltaTime, -rotLimit, rotLimit);
            body.localEulerAngles += mouseDelta.x * Time.deltaTime * Vector3.up;
            cam.transform.localEulerAngles = camRot;
        }
    }
}
