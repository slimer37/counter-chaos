using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace Interactables
{
    public class Button : MonoBehaviour, IInteractable, IStopInteractHandler
    {
        public UnityEvent onPress;
        public UnityEvent onRelease;

        [SerializeField] bool animate;
        [SerializeField] Vector3 pushAmount;
        [SerializeField] float animTime = 0.1f;
        [SerializeField] Ease ease = Ease.Linear;

        Vector3 originalPos;

        void Awake() => originalPos = transform.position;

        public void OnInteract(Transform sender)
        {
            if (animate)
                transform.DOMove(originalPos + transform.TransformDirection(pushAmount), animTime).SetEase(ease);
            
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
