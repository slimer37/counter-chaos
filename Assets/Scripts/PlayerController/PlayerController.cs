using DG.Tweening;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] InputProvider input;
    [SerializeField] CharacterController characterController;

    [Header("Animation")]
    [SerializeField] Animator meshAnimator;
    
    [Header("Movement")]
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] float slowSpeed;
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
    public float sensitivity;
    [SerializeField] float rotLimit;

    [Header("Bobbing")]
    [SerializeField] float bobAmount;
    [SerializeField] float bobTime;
    [SerializeField] float sprintBobTimeScale;
    [SerializeField] float bobResetSpeed;
    [SerializeField] Ease bobEase;

    float originalCamY;
    float originalFov;
    Tween bobTween;
    Tween returnTween;
    Tween fovTween;

    Sequence sprintTransition;
    bool isSprinting;
    bool isSlow;
    float currentSpeed;
    
    Vector2 mouseDelta;
    Vector3 camRot;
    Vector3 moveVector;

    float tempSensitivity;

    bool canMove = true;
    bool canLook = true;
    
    static readonly int Speed = Animator.StringToHash("speed");

    public const string SensitivityPrefKey = "Sensitivity";
    public const float DefaultSensitivity = 40;

    const float MouseScale = 0.05f;
    
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

    void OnMove(Vector2 moveInput)
    {
        if (!enabled) return;
        
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

    void OnJump()
    {
        if (!canMove || !characterController.isGrounded) return;
        moveVector.y = jumpForce;
    }
    
    void OnStartSprint() => OnSprint(true);
    void OnStopSprint() => OnSprint(false);

    void OnSprint(bool pressed)
    {
        if (isSlow) return;
        isSprinting = pressed;
    }
    
    void OnMoveMouse(Vector2 delta) => mouseDelta = sensitivity * MouseScale * delta;

    void Awake()
    {
        sensitivity = PlayerPrefs.GetFloat(SensitivityPrefKey, DefaultSensitivity);
        
        camRot = cam.transform.localEulerAngles;
        originalCamY = cam.transform.localPosition.y;
        originalFov = cam.fieldOfView;
        currentSpeed = walkSpeed;
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        input.Move += OnMove;
        input.MoveMouse += OnMoveMouse;
        input.StartSprint += OnStartSprint;
        input.CancelSprint += OnStopSprint;
        input.StartSlow += OnStartSlow;
        input.StopSlow += OnStopSlow;
        
        // Construct sprint transition.
        sprintTransition = DOTween.Sequence();
        sprintTransition.Append(DOTween.To(() => currentSpeed,
            newSpeed => currentSpeed = newSpeed, sprintSpeed, transitionTime));
        sprintTransition.Join(cam.DOFieldOfView(sprintFov, transitionTime));
        sprintTransition.Join(secondaryCam.DOFieldOfView(sprintFov, transitionTime));
        sprintTransition.SetEase(transitionEase).SetAutoKill(false).Pause();
    }

    void OnDestroy()
    {
        sprintTransition.Kill();
        
        
        input.Move -= OnMove;
        input.MoveMouse -= OnMoveMouse;
        input.StartSprint -= OnStartSprint;
        input.CancelSprint -= OnStopSprint;
        input.StartSlow -= OnStartSlow;
        input.StopSlow -= OnStopSlow;
    }

    void OnStartSlow() => OnSlow(true);
    void OnStopSlow() => OnSlow(false);

    void OnSlow(bool pressed)
    {
        if (!canMove || isSprinting || sprintTransition.IsPlaying()) return;
        
        isSlow = pressed;
        currentSpeed = isSlow ? slowSpeed : walkSpeed;
        UpdateBobbing();

        if (isSlow)
        {
            tempSensitivity = sensitivity;
            sensitivity /= 2;
        }
        else sensitivity = tempSensitivity;

        if (isSlow && isSprinting)
            fovTween = DOTween.Sequence()
                .Append(cam.DOFieldOfView(originalFov, transitionTime))
                .Join(secondaryCam.DOFieldOfView(originalFov, transitionTime));
        else if (!isSlow)
            fovTween?.Kill();
    }

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
