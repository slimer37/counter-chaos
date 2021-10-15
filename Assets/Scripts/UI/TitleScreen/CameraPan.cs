using DG.Tweening;
using UnityEngine;

namespace UI.TitleScreen
{
    public class CameraPan : MonoBehaviour
    {
        [SerializeField] Transform end;
        [SerializeField] Ease ease;
        [SerializeField] float duration;

        Sequence pan;

        void Start()
        {
            pan = DOTween.Sequence();
            pan.Append(transform.DOMove(end.position, duration).SetEase(ease));
            pan.Join(transform.DORotate(end.eulerAngles, duration).SetEase(ease));
            pan.SetLoops(-1, LoopType.Yoyo);
        }
    }
}
