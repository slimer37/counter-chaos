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

        [SerializeField] TextMeshProUGUI creditsText;
        [SerializeField] float scrollTime;
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

        void Awake() => rectTransform = creditsText.GetComponent<RectTransform>();

        void OnEnable()
        {
            rectTransform.pivot = new Vector2(rectTransform.pivot.x, 1);
            rectTransform.DOPivotY(-1, scrollTime).SetEase(ease);
        }
    }
}
