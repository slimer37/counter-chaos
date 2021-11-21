using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Core;
using DG.Tweening;

namespace UI.Tooltip
{
    [ExecuteInEditMode]
    internal class TooltipDisplay : Singleton<TooltipDisplay>
    {
        [SerializeField] TextMeshProUGUI title;
        [SerializeField] TextMeshProUGUI description;
        [SerializeField] int characterWrapLimit;
        [SerializeField] float fadeDuration;
        [SerializeField] Vector2 pivotAdjustmentMargin;
        [SerializeField] Vector2 defaultPivot;

        [SerializeField] CanvasGroup group;
        [SerializeField] LayoutElement layoutElement;
        
        Controls controls;
        RectTransform rectTransform;

        void Update()
        {
            if (Application.isPlaying || !layoutElement) return;
            layoutElement.enabled = description.text.Length > characterWrapLimit;
        }

        void OnValidate()
        {
            if (!group) return;
            group.interactable = false;
            group.blocksRaycasts = false;
        }

        void Awake()
        {
            OnValidate();
            rectTransform = GetComponent<RectTransform>();
            controls = new Controls();
            controls.Menu.CursorPosition.performed += RecordPosition;
        }

        public void Show(string titleText, string descText)
        {
            title.text = titleText;
            description.text = descText;

            title.gameObject.SetActive(titleText != "");
            description.gameObject.SetActive(descText != "");
            
            layoutElement.enabled = descText.Length > characterWrapLimit;
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            controls.Enable();
            group.DOFade(1, fadeDuration);
        }

        public void Hide()
        {
            controls.Disable();
            group.DOFade(0, fadeDuration);
        }

        void RecordPosition(InputAction.CallbackContext context)
        {
            var pos = context.ReadValue<Vector2>();
            transform.position = context.ReadValue<Vector2>();
		
            pos = (pos / new Vector2(Screen.width, Screen.height) - Vector2.one / 2) * 2;
            var absolutePos = new Vector2(Mathf.Abs(pos.x), Mathf.Abs(pos.y));
            var pivot = defaultPivot;
            if (absolutePos.x > 1 - pivotAdjustmentMargin.x)
            { pivot.x = pos.x > 0 ? 1 : 0; }
            if (absolutePos.y > 1 - pivotAdjustmentMargin.y)
            { pivot.y = pos.y > 0 ? 1 : 0; }
            rectTransform.pivot = pivot;
        }
    }
}
