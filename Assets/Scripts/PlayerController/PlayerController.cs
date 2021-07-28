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
        
        [Header("Jumping")]
        [SerializeField] float jumpForce;
        [SerializeField] float maxJumpTime;
        [SerializeField, Min(0)] float gravity = 9.81f;
        
        [Header("Looking")]
        [SerializeField] Camera cam;
        [SerializeField] Camera secondaryCam;
        [SerializeField] Transform body;
        [SerializeField] float sensitivity;
        [SerializeField] float rotLimit;

        Sequence sprintTransition;
        bool isSprinting;
        float currentSpeed;

        float jumpTime;
        bool isHoldingJump;
        
        Vector2 mouseDelta;
        Vector3 camRot;
        Vector3 moveVector;

        void OnMove(InputValue val)
        {
            var moveInput = val.Get<Vector2>();
            moveVector = new Vector3(moveInput.x, moveVector.y, moveInput.y);
        }

        void OnJump(InputValue val)
        {
            if (val.isPressed && characterController.isGrounded)
            {
                isHoldingJump = true;
                moveVector.y = jumpForce;
                jumpTime = maxJumpTime;
            }
            else if (!val.isPressed)
                isHoldingJump = false;
        }

        void OnSprint(InputValue val) => isSprinting = val.isPressed;
        void OnMoveMouse(InputValue val) => mouseDelta = sensitivity * val.Get<Vector2>();

        void Awake()
        {
            camRot = cam.transform.localEulerAngles;
            currentSpeed = walkSpeed;
            
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            
            // Construct sprint transition.
            sprintTransition = DOTween.Sequence();
            sprintTransition.Append(DOTween.To(() => currentSpeed,
                newSpeed => currentSpeed = newSpeed, sprintSpeed, transitionTime));
            sprintTransition.Join(cam.DOFieldOfView(sprintFov, transitionTime));
            sprintTransition.Join(secondaryCam.DOFieldOfView(sprintFov, transitionTime));
            sprintTransition.SetEase(transitionEase).SetAutoKill(false).Pause();
        }

        void OnDestroy() => sprintTransition.Kill();

        void Update()
        {
            // Jumping & Gravity

            if (isHoldingJump && jumpTime > 0)
                jumpTime -= Time.deltaTime;
            else if (!characterController.isGrounded)
                moveVector.y -= gravity * Time.deltaTime;
            
            // Looking
            
            characterController.Move(currentSpeed * Time.deltaTime * body.TransformDirection(moveVector));

            camRot.x = Mathf.Clamp(camRot.x - mouseDelta.y * Time.deltaTime, -rotLimit, rotLimit);
            body.localEulerAngles += mouseDelta.x * Time.deltaTime * Vector3.up;
            cam.transform.localEulerAngles = camRot;
            
            // Moving

            if (moveVector.z > 0)
            {
                if (sprintTransition.IsPlaying() && isSprinting) return;
            
                if (isSprinting)
                    sprintTransition.PlayForward();
                else
                    sprintTransition.PlayBackwards();
            }
            else if (!sprintTransition.isBackwards)
                sprintTransition.PlayBackwards();
        }
    }
}
