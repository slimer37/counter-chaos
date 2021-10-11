using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.TitleScreen
{
    public class StylizedButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public bool interactable = true;
        [SerializeField] Vector2 expandBy = new Vector2(50, 0);
        [SerializeField] float animDuration = 0.5f;
        [SerializeField] Ease ease = DOTween.defaultEaseType;
        [SerializeField] bool ignoreTimeScale;
	
        RectTransform rectTransform;
        Vector2 originalSizeDelta;

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            originalSizeDelta = rectTransform.sizeDelta;
        }

        public void OnPointerEnter(PointerEventData eventData) => Expand(true);
        public void OnPointerExit(PointerEventData eventData) => Expand(false);

        void Expand(bool value)
        {
            if (!interactable) return;
		
            rectTransform.DOKill();

            rectTransform.DOSizeDelta(originalSizeDelta + (value ? expandBy : Vector2.zero), animDuration)
                .SetEase(ease).SetUpdate(ignoreTimeScale);
        }
    }
}
