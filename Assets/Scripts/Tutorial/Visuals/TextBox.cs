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

        Controls controls;
        bool skipPressed;
        bool isDisplaying;

        void Awake()
        {
            canvasGroup.alpha = 0;
            controls = new Controls();
            controls.Gameplay.Jump.performed += _ => skipPressed = true;
        }

        void OnEnable() => controls.Enable();
        void OnDisable() => controls.Disable();
        void OnDestroy() => controls.Dispose();

        public void Display(params string[] text) => Display(text, false);
        
        public void Display(string[] text, bool autoClose) =>
            StartCoroutine(DisplayText(text, autoClose));

        public IEnumerator DisplayText(params string[] text) => DisplayText(text, false);

        public IEnumerator DisplayText(string[] text, bool autoClose)
        {
            if (isDisplaying) throw new InvalidOperationException("Text box is already being used.");
            isDisplaying = true;
            yield return canvasGroup.DOFade(1, fadeDuration).WaitForCompletion();
            
            foreach (var snippet in text)
            {
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

                if (!autoClose && snippet == text[text.Length - 1]) break;
                
                yield return new WaitUntil(() => skipPressed);
                skipPressed = false;
            }

            if (!autoClose) yield break;
            StopDisplaying();
        }

        public void StopDisplaying()
        {
            if (!isDisplaying) return;
            StopAllCoroutines();
            canvasGroup.DOFade(0, fadeDuration);
            isDisplaying = false;
        }
    }
}
