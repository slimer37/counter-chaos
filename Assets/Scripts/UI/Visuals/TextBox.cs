using System;
using System.Collections;
using Core;
using DG.Tweening;
using Input.Direct;
using TMPro;
using UnityEngine;

namespace UI.Visuals
{
    public class TextBox : MonoBehaviour
    {
        [SerializeField] InputProvider input;
        [SerializeField] TextMeshProUGUI textMesh;
        [SerializeField, Min(1)] float charsPerSec = 30;
        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField, Min(0)] float fadeDuration;
        [SerializeField] TextMeshProUGUI skipIndicator;

        bool skipPressed;
        bool isDisplaying;

        void Awake()
        {
            canvasGroup.alpha = 0;
            input.Jump += Skip;
            using var controls = new Controls();
            skipIndicator.text = controls.Gameplay.Jump.FormatDisplayString() + " to skip.";
        }
        
        void OnDestroy() => input.Jump -= Skip;

        void Skip() => skipPressed = true;

        IEnumerator PrepareToDisplay(string prepText)
        {
            isDisplaying = true;
            skipIndicator.gameObject.SetActive(false);
            
            // Set text so UI resizes
            textMesh.text = prepText;
            textMesh.maxVisibleCharacters = 0;
            
            // Fade in UI from 0
            canvasGroup.DOKill();
            canvasGroup.alpha = 0;
            yield return canvasGroup.DOFade(1, fadeDuration).WaitForCompletion();
        }

        IEnumerator DisplaySnippet(string snippet)
        {
            textMesh.text = snippet;
            textMesh.maxVisibleCharacters = 1;
            
            foreach (var _ in snippet)
            {
                yield return new WaitForSeconds(1 / charsPerSec);
                textMesh.maxVisibleCharacters++;
                
                // Exit when skip is pressed by checking after each character.
                if (skipPressed)
                {
                    textMesh.maxVisibleCharacters = snippet.Length;
                    skipPressed = false;
                    break;
                }
            }
        }

        IEnumerator WaitForSkip()
        {
            skipIndicator.gameObject.SetActive(true);
            yield return new WaitUntil(() => skipPressed);
            skipPressed = false;
            skipIndicator.gameObject.SetActive(false);
        }

        IEnumerator WaitForDisplay(bool closeWhenDone, params string[] snippets)
        {
            if (isDisplaying) throw new InvalidOperationException($"Text box {name} is already being used.");
            
            yield return PrepareToDisplay(snippets[0]);

            for (var i = 0; i < snippets.Length; i++)
            {
                yield return DisplaySnippet(snippets[i]);
                // On last snippet, don't show skip indicator if this dialogue does not close.
                if (i == snippets.Length - 1 && !closeWhenDone) break;
                yield return WaitForSkip();
            }

            if (closeWhenDone) EndDisplay();
        }
        
        public YieldInstruction Display(params string[] text) => Display(true, text);
        public YieldInstruction Display(bool closeWhenDone, params string[] text) => StartCoroutine(WaitForDisplay(closeWhenDone, text));

        public void EndDisplay()
        {
            if (!isDisplaying) return;
            StopAllCoroutines();
            canvasGroup.DOFade(0, fadeDuration);
            isDisplaying = false;
            skipPressed = false;
        }
    }
}
