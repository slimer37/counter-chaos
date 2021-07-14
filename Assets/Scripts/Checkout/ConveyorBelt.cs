using System.Collections.Generic;
using UnityEngine;

namespace Checkout
{
    public class ConveyorBelt : MonoBehaviour
    {
        [SerializeField] float speed = 1;
        [SerializeField] Rigidbody rb;
        
        [Header("Scrolling Texture")]
        [SerializeField] Renderer rend;
        [SerializeField] Vector2 scrollDirection = Vector2.up * 0.3f;

        List<CharacterController> collidingControllers = new List<CharacterController>();

        void Reset()
        {
            TryGetComponent(out rb);
            TryGetComponent(out rend);
        }

        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            collidingControllers.Add(other.GetComponent<CharacterController>());
        }

        void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            collidingControllers.Remove(other.GetComponent<CharacterController>());
        }

        void Awake() => scrollDirection *= rend.material.mainTextureScale;

        void FixedUpdate()
        {
            var delta = speed * Time.fixedDeltaTime * transform.forward;
            rb.position -= delta;
            rb.MovePosition(rb.position + delta);

            foreach (var controller in collidingControllers)
                controller.Move(delta);
            
            var offset = rend.material.mainTextureOffset;
            offset += speed * Time.fixedDeltaTime * scrollDirection;
            offset.x %= 1;
            offset.y %= 1;
            rend.material.mainTextureOffset = offset;
        }
    }
}
