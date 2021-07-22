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
        [SerializeField] float fadeDuration = 0.5f;
        [SerializeField] Ease ease = DOTween.defaultEaseType;
        [SerializeField] Graphic fadeBackground;
        [SerializeField] Ease fadeEase = Ease.OutSine;
        [SerializeField] bool ignoreTimeScale;
	
        RectTransform rectTransform;
        Vector2 originalSizeDelta;

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            originalSizeDelta = rectTransform.sizeDelta;
            if (interactable)
                fadeBackground.DOFade(0, 0);
        }

        public void OnPointerEnter(PointerEventData eventData) => Expand(true);
        public void OnPointerExit(PointerEventData eventData) => Expand(false);

        void Expand(bool value)
        {
            if (!interactable) return;
		
            rectTransform.DOKill();
            fadeBackground.DOKill();

            var sequence = DOTween.Sequence();

            sequence.Join(rectTransform.DOSizeDelta(originalSizeDelta + (value ? expandBy : Vector2.zero), animDuration)
                .SetEase(ease));
            sequence.Insert(value ? 0 : animDuration - fadeDuration,
                fadeBackground.DOFade(value ? 1 : 0, fadeDuration).SetEase(fadeEase));

            sequence.SetUpdate(ignoreTimeScale);
            sequence.Play();
        }
    }
}
