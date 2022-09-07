using DG.Tweening;
using UI.Settings.PlayerModifiers;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] CharacterController characterController;

    [Header("Animation")]
    [SerializeField] Animator meshAnimator;
    
    [Header("Movement")]
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] float transitionTime;
    [SerializeField] float sprintFov;
    [SerializeField] Ease transitionEase;
    
    [Header("Jumping")]
    [SerializeField] float jumpForce;
    [SerializeField, Min(0)] float gravity = 9.81f;
    
    [Header("Looking")]
    [SerializeField] Camera cam;
    [SerializeField] Camera secondaryCam;
    [SerializeField] Transform body;
    [SerializeField] float rotLimit;
    [SerializeField] float rotScale = 1;
    [SerializeField] InputActionReference lookAction;

    [Header("Bobbing")]
    [SerializeField] float bobAmount;
    [SerializeField] float bobTime;
    [SerializeField] float sprintBobTimeScale;
    [SerializeField] float bobResetSpeed;
    [SerializeField] Ease bobEase;

    float originalCamY;
    Tween bobTween;
    Tween returnTween;
    Tween fovTween;

    Sequence sprintTransition;
    bool isSprinting;
    bool isSlow;
    float currentSpeed;
    
    Vector3 camRot;
    Vector3 moveVector;

    float tempSensitivity;

    bool canMove = true;
    bool canLook = true;
    
    static readonly int Speed = Animator.StringToHash("speed");

    public bool UseGravity { get; set; } = true;

    public void EnableMovement(bool value)
    {
        canMove = value;
        if (canMove)
            UpdateBobbing();
        else
        {
            bobTween?.Kill();
            if (isSprinting)
                sprintTransition.PlayBackwards();
        }
    }
    
    public void EnableLook(bool value) => canLook = value;
    
    public void Suspend(bool value = true)
    {
        EnableMovement(!value);
        EnableLook(!value);
    }

    void OnMove(InputValue val)
    {
        if (!enabled) return;
        
        var moveInput = val.Get<Vector2>();
        moveVector = new Vector3(moveInput.x, moveVector.y, moveInput.y);

        if (!canMove) return;
        
        UpdateBobbing();
    }

    void UpdateBobbing()
    {
        var wasBobbing = bobTween != null && bobTween.active && bobTween.IsPlaying();
        var isBobbing = moveVector.x != 0 || moveVector.z != 0;
        if (isSlow) isBobbing = false;

        if (wasBobbing == isBobbing) return;
        
        bobTween?.Kill();
        returnTween?.Kill();
        
        var camT = cam.transform;
        var resetDuration = (originalCamY - camT.localPosition.y) / bobResetSpeed;
        
        returnTween = camT.DOLocalMoveY(originalCamY, resetDuration);
        
        if (!isBobbing) return;
        bobTween = cam.transform.DOLocalMoveY(originalCamY - bobAmount, bobTime)
            .SetDelay(resetDuration).SetLoops(-1, LoopType.Yoyo).SetEase(bobEase);
        if (isSprinting) bobTween.timeScale = sprintBobTimeScale;
    }

    void OnJump(InputValue val)
    {
        if (!canMove) return;
        if (val.isPressed)
        {
            if (characterController.isGrounded)
                moveVector.y = jumpForce;
        }
    }

    void OnSprint(InputValue val)
    {
        if (isSlow) return;
        isSprinting = val.isPressed;
    }

    void Awake()
    {
        camRot = cam.transform.localEulerAngles;
        originalCamY = cam.transform.localPosition.y;
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
        // Gravity
        
        if (UseGravity && !characterController.isGrounded)
            moveVector.y -= gravity * Time.deltaTime;
        
        // Moving

        var moveAmount = canMove ? body.TransformDirection(moveVector) : moveVector.y * Vector3.up;
        moveAmount.y /= currentSpeed;
        characterController.Move(currentSpeed * Time.deltaTime * moveAmount);
        
        moveAmount.y = 0;
        meshAnimator.SetFloat(Speed, moveAmount.sqrMagnitude);
        
        // Looking
        
        if (canLook)
        {
            var mouseDelta = SensitivitySlider.CurrentValue * rotScale * lookAction.action.ReadValue<Vector2>();
            camRot.x = Mathf.Clamp(camRot.x - mouseDelta.y, -rotLimit, rotLimit);
            body.localEulerAngles += mouseDelta.x * Vector3.up;
            cam.transform.localEulerAngles = camRot;
        }
        
        // Sprinting
        
        if (canMove && !isSlow)
        {
            if (moveVector.z > 0)
            {
                if (sprintTransition.IsPlaying() && isSprinting) return;

                if (bobTween.IsActive()) bobTween.timeScale = isSprinting ? sprintBobTimeScale : 1;

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
