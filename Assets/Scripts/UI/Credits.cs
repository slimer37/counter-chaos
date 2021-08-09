using System;
using System.Text.RegularExpressions;
using Core;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UI
{
    public class Credits : MonoBehaviour
    {
        [Serializable]
        struct Credit
        {
            public string role;
            public string name;
        }
        
        [SerializeField] CanvasGroup fadeGroup;
        [SerializeField] TextMeshProUGUI creditsText;
        [SerializeField] float scrollTime;
        [SerializeField] float fadeTime;
        [SerializeField] Ease ease;
        [SerializeField, Tooltip("{0-1}: Role, Name"), RequireSubstring("{0}", "{1}")] string creditFormat;
        
        [Header("Credits Text")]
        [SerializeField, TextArea] string beginningText;
        [SerializeField] Credit[] creditList;
        [SerializeField, TextArea] string endingText;

        Controls controls;
        Sequence scrollSequence;
        RectTransform creditsRect;

        void OnValidate()
        {
            if (!creditsText) return;

            creditsText.text = Regex.Unescape(beginningText) + "\n";
            foreach (var credit in creditList)
                creditsText.text += string.Format(Regex.Unescape(creditFormat), credit.role, credit.name) + "\n";
            creditsText.text += Regex.Unescape(endingText);
        }

        void Awake()
        {
            creditsRect = creditsText.GetComponent<RectTransform>();
            fadeGroup.alpha = 0;
            fadeGroup.blocksRaycasts = false;
            controls = new Controls();
            controls.Menu.Exit.performed += _ => Stop();
            
            // Construct scroll sequence.

            scrollSequence = DOTween.Sequence();
            scrollSequence.AppendCallback(() => {
                creditsRect.pivot = new Vector2(creditsRect.pivot.x, 1.5f);
                fadeGroup.blocksRaycasts = true;
            });
            scrollSequence.Append(fadeGroup.DOFade(1, fadeTime));
            scrollSequence.Append(creditsRect.DOPivotY(-0.5f, scrollTime).SetEase(ease));
            scrollSequence.AppendCallback(() => fadeGroup.blocksRaycasts = false);
            scrollSequence.Append(fadeGroup.DOFade(0, fadeTime));
            scrollSequence.SetAutoKill(false).Pause();
        }

        void OnEnable() => controls.Enable();
        void OnDisable() => controls.Disable();
        void OnDestroy()
        {
            scrollSequence.Kill();
            controls.Dispose();
        }

        void Stop()
        {
            if (scrollSequence.IsPlaying())
                scrollSequence.Complete(true);
        }

        public void ScrollCredits() => scrollSequence.Restart();
    }
}
