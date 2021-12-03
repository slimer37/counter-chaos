using DG.Tweening;
using UnityEngine;

namespace Animation
{
    public class TitleDoor : MonoBehaviour
    {
        [SerializeField] RectTransform rectT;
        [SerializeField] float delay;
        
        [Header("Before Up")]
        [SerializeField] bool pullYToZero = true;
        [SerializeField] float pullDownDuration;
        [SerializeField] float afterPullDelay;
        [SerializeField] Ease pullEase;
        
        [Header("Going Up")]
        [SerializeField] float upDuration;
        [SerializeField] Ease upEase;

        static bool doorPulledUp;
        
        float originalY;

        void Awake()
        {
            if (doorPulledUp)
            {
                rectT.gameObject.SetActive(false);
                return;
            }
            
            originalY = rectT.anchoredPosition.y;
            Animate();
        }
        
#if UNITY_EDITOR
        [ContextMenu("Redo Animation")]
        void AnimateTest()
        {
            if (!Application.isPlaying) return;

            rectT.anchoredPosition = new Vector2(0.5f, originalY);
            Animate();
        }
#endif
        
        void Animate()
        {
            rectT.pivot = new Vector2(0.5f, 1);
            rectT.gameObject.SetActive(true);

            var seq = DOTween.Sequence();

            if (pullYToZero)
            {
                seq.Append(rectT.DOAnchorPosY(0, pullDownDuration).SetEase(pullEase));
                seq.AppendInterval(afterPullDelay);
            }
            
            seq.Append(rectT.DOPivotY(0, upDuration).SetEase(upEase));

            seq.SetDelay(delay);
            
            doorPulledUp = true;
        }
    }
}
