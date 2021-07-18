using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace Interactables
{
    public class Button : MonoBehaviour, IInteractHandler, IStopInteractHandler
    {
        public UnityEvent onPress;
        public UnityEvent onRelease;

        [SerializeField] bool animate;
        [SerializeField] Vector3 pushAmount;
        [SerializeField] float animTime = 0.1f;
        [SerializeField] Ease ease = Ease.Linear;

        Vector3 originalPos;

        void OnDrawGizmosSelected()
        {
            if (!animate) return;
            Gizmos.color = Color.yellow;
            if (TryGetComponent(out MeshFilter filter))
                Gizmos.DrawMesh(filter.sharedMesh, transform.TransformPoint(pushAmount), transform.rotation);
            else
                Gizmos.DrawLine(transform.position, transform.TransformPoint(pushAmount));
        }

        void Awake() => originalPos = transform.position;

        public void OnInteract(Transform sender)
        {
            if (animate)
                transform.DOMove(transform.TransformPoint(pushAmount), animTime).SetEase(ease);
            
            onPress.Invoke();
        }
        
        public void OnStopInteract(Transform sender)
        {
            if (animate)
                transform.DOMove(originalPos, animTime).SetEase(ease);
            
            onRelease.Invoke();
        }
    }
}
