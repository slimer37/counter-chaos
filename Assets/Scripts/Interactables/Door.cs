using DG.Tweening;
using Interactables.Base;
using UnityEngine;

namespace Interactables
{
    public class Door : MonoBehaviour, IInteractHandler
    {
        [SerializeField] float rotationAmount;
        [SerializeField] float rotationTime;
        [SerializeField, Min(0)] Vector3 rotationAxis = Vector3.up;

        bool open;
        Tween openTween;

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            var multiplier = rotationAmount < 0 ? -1 : 1;
            for (var i = 0; i < Mathf.Abs(rotationAmount); i++)
                Gizmos.DrawRay(transform.position, Quaternion.AngleAxis(multiplier * i, rotationAxis) * transform.forward);
            
            Gizmos.DrawRay(transform.position, Quaternion.AngleAxis(rotationAmount, rotationAxis) * transform.forward);
        }

        void Awake()
        {
            openTween = transform.DOLocalRotateQuaternion(Quaternion.AngleAxis(rotationAmount, rotationAxis), rotationTime);
            openTween.Pause();
            openTween.SetAutoKill(false);
        }

        void OnDestroy() => openTween.Kill();

        public void OnInteract(Transform sender) => ToggleOpen();

        void ToggleOpen()
        {
            open = !open;
            
            if (open)
                openTween.PlayForward();
            else
                openTween.PlayBackwards();
        }
    }
}
