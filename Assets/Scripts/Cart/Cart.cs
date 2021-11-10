using UnityEngine;
using Interactables.Base;

public class Cart : MonoBehaviour, IInteractHandler
{
    [SerializeField] float holdDistance;
    [SerializeField] Rigidbody rb;

    Transform track;
    bool isBeingPushed;

    public void OnInteract(Transform sender)
    {
        isBeingPushed = !isBeingPushed;
        track = sender;
    }

    void FixedUpdate()
    {
        if (!isBeingPushed) return;

        var pos = track.position;
        pos.y = transform.position.y;
        
        rb.MovePosition(pos + transform.forward * holdDistance);
    }
}
