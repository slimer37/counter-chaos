using System;
using System.Collections;
using Core;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Tutorial.Visuals
{
    public class TextBox : MonoBehaviour
    {
        [SerializeField, TextArea] string[] test;
        [SerializeField] TextMeshProUGUI textMesh;
        [SerializeField, Min(1)] float charsPerSec = 30;
        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField, Min(0)] float fadeDuration;

        public static TextBox Instance { get; private set; }

        Controls controls;
        bool skipPressed;
        bool isDisplaying;

        public void Display(params string[] text)
        {
            if (isDisplaying) throw new InvalidOperationException("Text box is already being used.");
            StartCoroutine(DisplayText(text));
        }

        void Awake()
        {
            canvasGroup.alpha = 0;
            controls = new Controls();
            controls.Gameplay.Jump.performed += _ => skipPressed = true;
            
            Instance = this;
            Display(test);
        }

        void OnEnable() => controls.Enable();
        void OnDisable() => controls.Disable();
        void OnDestroy() => controls.Dispose();

        IEnumerator DisplayText(string[] text)
        {
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
                
                yield return new WaitUntil(() => skipPressed);
                skipPressed = false;
            }
            
            yield return canvasGroup.DOFade(0, fadeDuration).WaitForCompletion();
            isDisplaying = false;
        }
    }
}
