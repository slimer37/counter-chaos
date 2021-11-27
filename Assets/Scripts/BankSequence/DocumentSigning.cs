using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BankSequence
{
    public class DocumentSigning : MonoBehaviour
    {
        [SerializeField] Transform cam;
        [SerializeField] Vector3 expectedFacingDirection;
        [SerializeField, Range(-1, 1)] float dotProductMinimum;
        [SerializeField] TMP_Text documentText;
        [SerializeField] Transform upPosition;
        [SerializeField] float focusDuration;
        [SerializeField] Ease ease;

        int nameIndex;
        
        string signature = "";
        string originalText;

        bool ableToSign;

        Keyboard keyboard;

        Sequence upSequence;

        void OnDrawGizmosSelected()
        {
            if (!cam || expectedFacingDirection.sqrMagnitude == 0) return;
            Gizmos.DrawRay(cam.position, expectedFacingDirection.normalized);
        }

        void Awake()
        {
            expectedFacingDirection.Normalize();
            
            originalText = documentText.text = documentText.text.Replace("{0}", DateTime.Now.ToLongDateString());
            nameIndex = originalText.IndexOf("{1}", StringComparison.Ordinal);
            originalText = originalText.Replace("{1}", "");
            UpdateText();
            
            keyboard = Keyboard.current;
            keyboard.onTextInput += OnTextInput;

            upSequence = DOTween.Sequence();
            upSequence.Append(transform.DOMove(upPosition.position, focusDuration).SetEase(ease));
            upSequence.Join(transform.DORotate(upPosition.eulerAngles, focusDuration).SetEase(ease));
            upSequence.Pause().SetAutoKill(false);
        }

        void Update()
        {
            ableToSign = Vector3.Dot(cam.forward, expectedFacingDirection) > dotProductMinimum;
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
            if (!ableToSign || char.IsControl(c)) return;
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
