using Interactables.Base;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Checkout.RegisterInput
{
    public class RegisterIDEntry : MonoBehaviour, IInteractHandler, IStopInteractHandler
    {
        [SerializeField] Hoverable hoverable;
        [SerializeField] UnityEvent<char?> onNumberInput;

        bool enteringID;
        PlayerController.PlayerController playerController;

        void InputID(char c)
        {
            if (!enteringID || !char.IsNumber(c)) return;
            onNumberInput.Invoke(c);
        }

        void Update()
        {
            if (!enteringID) return;
            if (Keyboard.current.backspaceKey.wasPressedThisFrame)
                onNumberInput.Invoke(null);
        }

        void OnEnable()
        {
            Keyboard.current.onTextInput += InputID;
            hoverable.enabled = true;
        }
    
        void OnDisable()
        {
            Keyboard.current.onTextInput -= InputID;
            hoverable.enabled = false;
        }

        public void OnInteract(Transform sender)
        {
            enteringID = true;
        
            playerController = sender.GetComponent<PlayerController.PlayerController>();
            playerController.enabled = false;
        }
    
        public void OnStopInteract(Transform sender)
        {
            if (!playerController) return;
        
            enteringID = false;
        
            playerController.enabled = true;
            playerController = null;
        }
    }
}
