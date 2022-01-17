using UnityEngine;
using UnityEngine.InputSystem;
using Interactables.Base;
using Core;

public class Cart : MonoBehaviour, IInteractHandler
{
    [SerializeField] float rotSpeed;
    [SerializeField] float moveSpeed;
    [SerializeField] Hoverable hoverable;
    [SerializeField] Rigidbody rb;
    
    [Header("Player Positioning")]
    [SerializeField] float playerStartHeight = 1;
    [SerializeField] float holdDistance;

    [Header("Cart Movement Inhibition")]
    [SerializeField] LayerMask mask;
    [SerializeField] Vector3 playerCheckBox;
    [SerializeField] Vector3 playerCheckOffset;
    [SerializeField] Vector3 forwardBox;
    [SerializeField] Vector3 forwardBoxCenter;

    bool isBeingPushed;
    Controls controls;
    Vector3 moveDirection;
    Vector3 moveRotation;
    PlayerController controller;
    
    Vector3 moveDelta;
    float upOffset;

    void OnDrawGizmosSelected()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(playerCheckOffset, playerCheckBox);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(forwardBoxCenter, forwardBox);
    }

    void Awake()
    {
        upOffset = playerStartHeight - transform.position.y;
        
        controls = new Controls();
        controls.Gameplay.Slow.performed += Exit;
        controls.Gameplay.Move.performed += Move;
        controls.Gameplay.Move.canceled += Move;
        controls.Enable();
        
        hoverable.RegisterPriorityCheck(_ => !isBeingPushed, 0);
    }
    
    void Move(InputAction.CallbackContext ctx)
    {
        if (!isBeingPushed) return;
        var dir = ctx.ReadValue<Vector2>();

        moveDirection.z = dir.y * moveSpeed;
        moveRotation.y = dir.x * rotSpeed;
    }

    void Exit(InputAction.CallbackContext ctx)
    {
        if (!isBeingPushed) return;
        Setup(false);

        moveDirection = default;
        moveRotation = default;
    }

    public void OnInteract(Transform sender)
    {
        if (isBeingPushed) return;
        
        controller ??= sender.GetComponent<PlayerController>();
        Setup(true);
    }

    void Setup(bool value)
    {
        isBeingPushed = value;
        controller.EnableMovement(!value);
        controller.UseGravity = !value;

        rb.collisionDetectionMode = value ? CollisionDetectionMode.Continuous : CollisionDetectionMode.Discrete;
    }

    void FixedUpdate()
    {
        if (!isBeingPushed) return;
        
        var dir = moveDirection;

        if (Physics.CheckBox(transform.TransformPoint(playerCheckOffset), playerCheckBox / 2, transform.rotation, mask))
            dir.z = Mathf.Max(moveSpeed, dir.z);

        if (Physics.CheckBox(transform.TransformPoint(forwardBoxCenter), forwardBox / 2, transform.rotation, mask))
            dir.z = Mathf.Min(0, dir.z);

        dir = transform.TransformDirection(dir);
        rb.MovePosition(rb.position + dir * Time.fixedDeltaTime);
        
        // Rotate player with cart
        var rot = rb.rotation.eulerAngles + moveRotation * Time.fixedDeltaTime;
        rb.MoveRotation(Quaternion.Euler(rot));
        controller.transform.Rotate(moveRotation * Time.fixedDeltaTime);
        
        // Set player position behind holdDistance units and at floor level (defined by upOffset)
        var pos = rb.position - transform.forward * holdDistance;
        pos.y += upOffset;
        controller.transform.position = pos;
    }
}
