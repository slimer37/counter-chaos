using System;
using System.Collections;
using Core;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Tutorial.Visuals
{
    public class TextBox : Singleton<TextBox>
    {
        [SerializeField] TextMeshProUGUI textMesh;
        [SerializeField, Min(1)] float charsPerSec = 30;
        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField, Min(0)] float fadeDuration;
        [SerializeField] TextMeshProUGUI skipIndicator;

        Controls controls;
        bool skipPressed;
        bool isDisplaying;

        void Awake()
        {
            canvasGroup.alpha = 0;
            controls = new Controls();
            controls.Gameplay.Jump.performed += _ => skipPressed = true;
            skipIndicator.text = controls.Gameplay.Jump.FormatDisplayString() + " to skip.";
        }

        void OnEnable() => controls.Enable();
        void OnDisable() => controls.Disable();
        void OnDestroy() => controls.Dispose();

        public YieldInstruction Display(params string[] text) => Display(false, text);
        public YieldInstruction Display(bool closable, params string[] text) => StartCoroutine(WaitForDisplay(closable, text));

        IEnumerator WaitForDisplay(bool closable, params string[] text)
        {
            if (isDisplaying) throw new InvalidOperationException("Text box is already being used.");
            isDisplaying = true;
            skipIndicator.gameObject.SetActive(false);
            
            canvasGroup.DOKill();
            if (canvasGroup.alpha == 0)
                canvasGroup.alpha = 1;
            else if (canvasGroup.alpha < 1)
                yield return canvasGroup.DOFade(1, fadeDuration).WaitForCompletion();
            
            foreach (var snippet in text)
            {
                skipIndicator.gameObject.SetActive(false);
                
                textMesh.text = snippet;
                textMesh.maxVisibleCharacters = 1;
                for (var i = 1; i <= snippet.Length; i++)
                {
                    yield return new WaitForSeconds(1 / charsPerSec);
                    textMesh.maxVisibleCharacters = i;

                    if (skipPressed)
                    {
                        textMesh.maxVisibleCharacters = snippet.Length;
                        skipPressed = false;
                        break;
                    }
                }

                if (!closable && snippet == text[text.Length - 1]) break;
                
                skipIndicator.gameObject.SetActive(true);
                yield return new WaitUntil(() => skipPressed);
                skipPressed = false;
            }

            if (!closable) yield break;
            Clear();
        }

        public void Clear()
        {
            if (!isDisplaying) return;
            StopAllCoroutines();
            canvasGroup.DOFade(0, fadeDuration);
            isDisplaying = false;
            skipPressed = false;
        }
    }
}
