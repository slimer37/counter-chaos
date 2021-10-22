using Core;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Tutorial.Visuals
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

        public LocatorHint Instance { get; private set; }

        void Awake()
        {
            Instance = this;
            
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

            var point = cam.WorldToScreenPoint(pos);
            
            // If the object is more than 90 degrees out of view, move locator to left or right edge of screen.
            if (point.z < 0)
            {
                point -= new Vector3(Screen.width, Screen.height);
                point.x = point.x < 0 ? Screen.width : 0;
                point.y *= -1;
            }

            point.x = Mathf.Clamp(point.x, 0, Screen.width);
            point.y = Mathf.Clamp(point.y, 0, Screen.height - yOffset);
            
            canvasGroup.alpha = 1;
            rootContainer.position = point + Vector3.up * yOffset;
        }
        
        void Update()
        {
            if (!showing) return;
            if (followTarget) PositionAt(followTarget.position);
        }
    }
}
