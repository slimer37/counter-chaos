using System.Collections.Generic;
using UnityEngine;

namespace Register
{
    public class ConveyorBelt : MonoBehaviour
    {
        [SerializeField] float speed;
        [SerializeField] Rigidbody rb;

        List<CharacterController> collidingControllers = new List<CharacterController>();

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

        void FixedUpdate()
        {
            var delta = speed * Time.fixedDeltaTime * transform.forward;
            rb.position -= delta;
            rb.MovePosition(rb.position + delta);

            foreach (var controller in collidingControllers)
                controller.Move(delta);
        }
    }
}
