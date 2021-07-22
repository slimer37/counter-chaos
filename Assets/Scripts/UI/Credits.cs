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
        [SerializeField] Credit[] creditList;
        
        RectTransform rectTransform;

        void OnValidate()
        {
            if (!creditsText) return;

            creditsText.text = "";
            foreach (var credit in creditList)
                creditsText.text += string.Format(Regex.Unescape(creditFormat), credit.role, credit.name) + "\n";
        }

        void Awake()
        {
            rectTransform = creditsText.GetComponent<RectTransform>();
            fadeGroup.alpha = 0;
        }

        public void ScrollCredits()
        {
            rectTransform.pivot = new Vector2(rectTransform.pivot.x, 1);
            var sequence = DOTween.Sequence();
            fadeGroup.blocksRaycasts = true;
            sequence.Append(fadeGroup.DOFade(1, fadeTime));
            sequence.Append(rectTransform.DOPivotY(-1, scrollTime).SetEase(ease));
            sequence.AppendCallback(() => fadeGroup.blocksRaycasts = false);
            sequence.Append(fadeGroup.DOFade(0, fadeTime));
        }
    }
}
