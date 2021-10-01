using Core;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Visuals
{
    public class LocatorHint : Singleton<LocatorHint>
    {
        [SerializeField] RectTransform rootContainer;
        [SerializeField] RectTransform contentParent;
        [SerializeField] TextMeshProUGUI text;
        [SerializeField] float yOffset;
        [Header("Bouncing")]
        [SerializeField] float bounceY;
        [SerializeField] float bounceDuration;
        [SerializeField] Ease ease;
        
        CanvasGroup canvasGroup;
        Transform followTarget;
        bool showing;

        void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            contentParent.DOLocalMoveY(bounceY, bounceDuration).SetEase(ease).SetLoops(-1, LoopType.Yoyo);
        }
        
        public void ShowAt(Vector3 position, string message)
        {
            Show(message);
            PositionAt(position);
        }
        
        public void ShowAndFollow(Transform target, string message)
        {
            Show(message);
            followTarget = target;
        }

        void Show(string message)
        {
            showing = true;
            text.text = message;
        }

        public void Hide()
        {
            showing = false;
            canvasGroup.alpha = 0;
        }
        
        void PositionAt(Vector3 pos)
        {
            var cam = Player.Camera;
            
            // If the object is out of view, hide the hint.
            if (Vector3.Dot(cam.transform.forward, followTarget.position - cam.transform.position) < 0)
            {
                canvasGroup.alpha = 0;
                return;
            }
            
            canvasGroup.alpha = 1;
            rootContainer.position = cam.WorldToScreenPoint(pos) + Vector3.up * yOffset;
        }
        
        void Update()
        {
            if (!showing) return;
            if (followTarget) PositionAt(followTarget.position);
        }
    }
}
