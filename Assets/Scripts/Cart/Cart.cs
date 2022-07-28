using UnityEngine;
using Interactables.Base;
using DG.Tweening;

public class Cart : MonoBehaviour, IInteractable
{
    [SerializeField] float rotSpeed;
    [SerializeField] float moveSpeed;
    [SerializeField] Rigidbody rb;
    [SerializeField] InputProvider input;
    
    [Header("Player Positioning")]
    [SerializeField] float playerStartHeight = 1;
    [SerializeField] float holdDistance;

    [Header("Cart Movement Inhibition")]
    [SerializeField] LayerMask mask;
    [SerializeField] Vector3 playerCheckBox;
    [SerializeField] Vector3 playerCheckOffset;
    [SerializeField] Vector3 forwardBox;
    [SerializeField] Vector3 forwardBoxCenter;

    [Header("Animation")]
    [SerializeField] float duration;
    [SerializeField] Vector3 strength;
    [SerializeField] int vibrato;
    [SerializeField] Transform animateCart;
    [SerializeField] bool fadeOut;

    [Header("Container")]
    [SerializeField] Transform container;

    bool isBeingPushed;
    Vector3 moveDirection;
    Vector3 moveRotation;
    PlayerController controller;
    
    Vector3 moveDelta;
    float upOffset;

    Vector3 containerPos;

    Tween shake;
    Quaternion originalCartRot;

    void OnDrawGizmosSelected()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(playerCheckOffset, playerCheckBox);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(forwardBoxCenter, forwardBox);
    }

    void OnValidate()
    {
        if (container && container.parent != transform)
            Debug.LogWarning("Container should be a child of the cart object.");
    }

    public bool CanInteract(Transform sender) => !isBeingPushed;

    void Awake()
    {
        originalCartRot = animateCart.localRotation;
        shake = animateCart.DOShakeRotation(duration, strength, vibrato, fadeOut: fadeOut).SetLoops(-1).SetAutoKill(false).Pause();
        
        upOffset = playerStartHeight - transform.position.y;
        
        input.StartSlow += Exit;
        input.Move += Move;

        containerPos = container.transform.localPosition;
        container.transform.parent = null;
        
        rb.maxAngularVelocity = 0;
    }

    void OnDestroy()
    {
        input.StartSlow -= Exit;
        input.Move -= Move;
    }

    void Move(Vector2 dir)
    {
        if (!isBeingPushed) return;

        moveDirection.z = dir.y * moveSpeed;
        moveRotation.y = dir.x * rotSpeed;
    }

    void Exit()
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
        if (!isBeingPushed)
        {
            container.transform.position = transform.TransformPoint(containerPos);
            container.transform.rotation = transform.rotation;
            return;
        }
        
        var dir = moveDirection;
        
        // Move cart forward if back collider is in wall
        if (Physics.CheckBox(transform.TransformPoint(playerCheckOffset), playerCheckBox / 2, transform.rotation, mask))
            dir.z = Mathf.Max(moveSpeed / 2, dir.z);
        
        // Stop cart moving forward if front collider is in wall
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
        
        container.transform.position = transform.TransformPoint(containerPos);
        container.transform.rotation = transform.rotation;
        
        rb.velocity = Vector3.zero;

        if (dir.z != 0)
        {
            if (!shake.IsPlaying())
                shake.Play();
        }
        else
        {
            if (shake.IsPlaying())
            {
                shake.Pause();
                animateCart.localRotation = originalCartRot;
            }
        }
    }
}
