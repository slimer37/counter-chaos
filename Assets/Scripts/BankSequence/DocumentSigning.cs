using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BankSequence
{
    public class DocumentSigning : MonoBehaviour
    {
        [SerializeField] Camera cam;
        
        [Header("Text")]
        [SerializeField] TMP_Text documentText;
        [SerializeField, Min(0)] int charLimit;
        
        [Header("Animation")]
        [SerializeField] Transform upPosition;
        [SerializeField] Transform pen;
        [SerializeField] Transform penUpPosition;
        [SerializeField] float focusDuration;
        [SerializeField] Ease ease;

        int nameIndex;
        
        string signature = "";
        string originalText;

        bool ableToSign;

        Keyboard keyboard;

        Sequence upSequence;

        void Awake()
        {
            originalText = documentText.text = documentText.text.Replace("{0}", DateTime.Now.ToLongDateString());
            nameIndex = originalText.IndexOf("{1}", StringComparison.Ordinal);
            originalText = originalText.Replace("{1}", "");
            UpdateText();
            
            keyboard = Keyboard.current;
            keyboard.onTextInput += OnTextInput;

            upSequence = DOTween.Sequence();
            AddTweenTransform(transform, upPosition);
            if (pen) AddTweenTransform(pen, penUpPosition);
            upSequence.Pause().SetAutoKill(false);
            upSequence.isBackwards = true;

            void AddTweenTransform(Transform obj, Transform to)
            {
                upSequence.Join(obj.DOMove(to.position, focusDuration).SetEase(ease));
                upSequence.Join(obj.DORotate(to.eulerAngles, focusDuration).SetEase(ease));
            }
        }

        void Update()
        {
            ableToSign = Physics.Raycast(cam.ViewportPointToRay(new Vector3(0.5f, 0.5f)), out var hit)
                         && hit.transform == transform;
            Focus(ableToSign);
            
            if (!ableToSign || signature.Length == 0) return;
            if (keyboard.backspaceKey.wasPressedThisFrame)
            {
                signature = signature.Remove(signature.Length - 1);
                UpdateText();
            }
        }
        
        void OnTextInput(char c)
        {
            if (!ableToSign || signature.Length >= charLimit || char.IsControl(c)) return;
            signature += c;
            UpdateText();
        }

        void UpdateText() => documentText.text = originalText.Insert(nameIndex, signature);

        void Focus(bool up)
        {
            if (up != upSequence.isBackwards) return;
            
            if (up) upSequence.PlayForward();
            else upSequence.PlayBackwards();
        }
    }
}
