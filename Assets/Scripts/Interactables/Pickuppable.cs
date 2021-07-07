using DG.Tweening;
using Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Interactables
{
    public sealed class Pickuppable : MonoBehaviour, IInteractable
    {
        [SerializeField] Hoverable hoverable;
        [SerializeField] Rigidbody rb;

        bool isHeld;

        const float PickupTime = 0.8f;

        static bool playerIsHoldingObject;
        static Controls controls;

        [RuntimeInitializeOnLoadMethod]
        static void InitControls()
        {
            controls = new Controls();
            controls.Enable();
        }
        
        public void OnInteract(Transform player)
        {
            if (playerIsHoldingObject) return;

            hoverable.OnHoverExit();
            Setup(player);
            
            transform.DOLocalMove(holdPosition, PickupTime);
            transform.DOLocalRotateQuaternion(Quaternion.identity, PickupTime);
        }

        void Drop(InputAction.CallbackContext ctx) => Setup(null);

        void Setup(Transform holder)
        {
            transform.parent = holder;
            isHeld = holder;
            rb.isKinematic = holder;
            playerIsHoldingObject = holder;
            hoverable.enabled = !holder;

            if (holder)
                controls.Gameplay.Drop.performed += Drop;
            else
                controls.Gameplay.Drop.performed -= Drop;
        }
    }
}
