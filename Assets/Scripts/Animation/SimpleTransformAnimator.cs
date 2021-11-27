using DG.Tweening;
using UnityEngine;

namespace Animation
{
    public class SimpleTransformAnimator : MonoBehaviour
    {
        [SerializeField] new Transform transform;
        [SerializeField] bool startOnAwake = true;
        [SerializeField] bool local;
        [SerializeField] bool dontChangePosition;
        [SerializeField] Vector3 position;
        [SerializeField] bool dontChangeRotation;
        [SerializeField] Vector3 rotation;
        [SerializeField] float duration;
        [SerializeField] bool loop;
        [SerializeField] LoopType type;
        [SerializeField] Ease ease;

        Sequence sequence;

        void Awake()
        {
            sequence = DOTween.Sequence();
            if (!dontChangePosition) sequence.Join(local ? transform.DOLocalMove(position, duration) : transform.DOMove(position, duration));
            if (!dontChangeRotation) sequence.Join(local ? transform.DOLocalRotate(rotation, duration) : transform.DORotate(rotation, duration));
            sequence.SetEase(ease).SetAutoKill(false);
            if (loop) sequence.SetLoops(-1, type);

            if (!startOnAwake)
                sequence.Pause();
        }

        public void Animate()
        {
            sequence.Play();
        }
    }
}
