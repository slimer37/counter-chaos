using UnityEngine;
using UnityEngine.InputSystem;

namespace Project
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] CharacterController characterController;
        
        [Header("Movement")]
        [SerializeField] float walkSpeed;
        [SerializeField] float sprintSpeed;
        
        [Header("Looking")]
        [SerializeField] new Transform camera;
        [SerializeField] Transform body;
        [SerializeField] float sensitivity;
        [SerializeField] float rotLimit;

        bool isSprinting;
        
        Vector2 mouseDelta;
        Vector3 camRot;
        Vector3 moveVector;

        void OnMove(InputValue val)
        {
            var moveInput = val.Get<Vector2>();
            moveVector = new Vector3(moveInput.x, moveVector.y, moveInput.y);
        }
        
        void OnSprint(InputValue val) => isSprinting = val.isPressed;
        void OnMoveMouse(InputValue val) => mouseDelta = sensitivity * val.Get<Vector2>();

        void Awake() => camRot = camera.localEulerAngles;

        void Start()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        void Update()
        {
            if (!characterController.isGrounded)
                moveVector.y -= 9.81f * Time.deltaTime;
            
            var currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
            characterController.Move(currentSpeed * Time.deltaTime * body.TransformDirection(moveVector));

            camRot.x = Mathf.Clamp(camRot.x - mouseDelta.y * Time.deltaTime, -rotLimit, rotLimit);
            body.localEulerAngles += mouseDelta.x * Time.deltaTime * Vector3.up;
            camera.localEulerAngles = camRot;
        }
    }
}
