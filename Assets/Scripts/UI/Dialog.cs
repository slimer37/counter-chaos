using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Dialog : MonoBehaviour
    {
        [SerializeField] CanvasGroup dialogBoxAndPanel;
        [SerializeField] RectTransform dialogBox;
        [SerializeField] GameObject yesNoButtons;
        [SerializeField] GameObject okButton;
        [SerializeField] TextMeshProUGUI dialogMessage;
        [SerializeField] TextMeshProUGUI dialogTitle;
        [SerializeField] Image dialogTopBar;
        [SerializeField] float fadeTime;

        Action<bool> onAnswerReceived;

        public static Dialog Instance { get; private set; }
	
        Color defaultBarColor;
        bool affectedTimeScale;

        void Awake()
        {
            Instance = this;
            defaultBarColor = dialogTopBar.color;
        }

        public void YesNo(string title, string message, Action<bool> onAnswered, Color? color = null, bool affectTimeScale = true) =>
            ShowDialog(color, yesNoButtons, title, message, onAnswered, affectTimeScale);

        public void Notify(string title, string message, Action onConfirmed, Color? color = null, bool affectTimeScale = true) =>
            ShowDialog(color, okButton, title, message, _ => onConfirmed(), affectTimeScale);

        void ShowDialog(Color? color, GameObject buttonSet, string title, string message, Action<bool> onFinished, bool affectTimeScale)
        {
            affectedTimeScale = affectTimeScale;
            if (affectTimeScale)
                Time.timeScale = 0;
		
            onAnswerReceived = onFinished;
		
            dialogTopBar.color = color ?? defaultBarColor;
            yesNoButtons.SetActive(false);
            okButton.SetActive(false);
            buttonSet.SetActive(true);
		
            dialogTitle.text = title;
            dialogMessage.text = message;
		
            dialogBoxAndPanel.gameObject.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(dialogBox);
		
            dialogBoxAndPanel.DOFade(1, fadeTime).SetUpdate(true);
        }

        public void AnswerYesNo(bool yes)
        {
            if (!dialogBoxAndPanel.gameObject.activeSelf) throw new Exception($"{nameof(AnswerYesNo)} called illegally (Dialog box not open).");
		
            if (affectedTimeScale)
                Time.timeScale = 1;
		
            dialogBoxAndPanel.DOFade(0, fadeTime).OnComplete(() => dialogBoxAndPanel.gameObject.SetActive(false));
            onAnswerReceived(yes);
        }
    }
}
