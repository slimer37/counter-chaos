using Interactables.Base;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Interactables
{
    public class TextEntry : MonoBehaviour, IInteractHandler
    {
        [SerializeField] Hoverable hoverable;
        [SerializeField] TextMeshPro text;
        [SerializeField, Min(1)] int charLimit = 1;

        bool enteringText;
        PlayerController tempController;

        void InputID(char c)
        {
            if (!enteringText || text.text.Length >= charLimit || char.IsControl(c)) return;
            text.text += c;
        }

        void Update()
        {
            if (!enteringText || text.text.Length == 0) return;

            var keyboard = Keyboard.current;

            if (keyboard.backspaceKey.wasPressedThisFrame)
                text.text = text.text.Remove(text.text.Length - 1);

            if (keyboard.enterKey.wasPressedThisFrame)
            {
                enteringText = false;
                tempController.Suspend(false);
            }
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
            enteringText = true;
            (tempController = sender.GetComponent<PlayerController>()).Suspend();
        }
    }
}
