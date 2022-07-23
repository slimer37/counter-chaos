using Interactables.Base;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Checkout.RegisterInput
{
    public class RegisterIDEntry : MonoBehaviour, IInteractable
    {
        [SerializeField] UnityEvent<char?> onNumberInput;
        [SerializeField] UnityEvent onSubmitInput;

        bool enteringID;
        PlayerController tempController;

        void InputID(char c)
        {
            if (!enteringID || !char.IsNumber(c)) return;
            onNumberInput.Invoke(c);
        }

        void Update()
        {
            if (!enteringID) return;

            var keyboard = Keyboard.current;
            
            if (keyboard.backspaceKey.wasPressedThisFrame)
                onNumberInput.Invoke(null);
            
            if (keyboard.enterKey.wasPressedThisFrame)
                onSubmitInput.Invoke();
                
        }

        void OnEnable() => Keyboard.current.onTextInput += InputID;
        void OnDisable() => Keyboard.current.onTextInput -= InputID;

        public void OnInteract(Transform sender)
        {
            enteringID = true;
            (tempController = sender.GetComponent<PlayerController>()).Suspend();
        }

        public void OnStopInteract(Transform sender)
        {
            if (!tempController) return;
            enteringID = false;
            tempController.Suspend(false);
        }
    }
}
