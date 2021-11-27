using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BankSequence
{
    public class DocumentSigning : MonoBehaviour
    {
        [SerializeField] Chair chair;
        [SerializeField] TMP_Text documentText;

        int nameIndex;
        
        string signature = "";
        string originalText;

        Keyboard keyboard;
        
        void Awake()
        {
            originalText = documentText.text = documentText.text.Replace("{0}", DateTime.Now.ToLongDateString());
            nameIndex = originalText.IndexOf("{1}", StringComparison.Ordinal);
            originalText = originalText.Replace("{1}", "");
            UpdateText();
            
            keyboard = Keyboard.current;
            keyboard.onTextInput += OnTextInput;
        }

        void Update()
        {
            if (signature.Length == 0) return;
            if (keyboard.backspaceKey.wasPressedThisFrame)
            {
                signature = signature.Remove(signature.Length - 1);
                UpdateText();
            }
        }
        
        void OnTextInput(char c)
        {
            if (char.IsControl(c)) return;
            signature += c;
            UpdateText();
        }

        void UpdateText() => documentText.text = originalText.Insert(nameIndex, signature);
    }
}
