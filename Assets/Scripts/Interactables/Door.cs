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
            var col = Color.blue;
            col.a = 0.5f;
            Gizmos.color = col;
            
            var position = transform.position;
            var rotation = Quaternion.Euler(transform.eulerAngles + rotationAxis * rotationAmount);
            var meshFilter = GetComponent<MeshFilter>();
            if (!meshFilter)
            {
                meshFilter = GetComponentInChildren<MeshFilter>();
                position = transform.TransformPoint(rotation * meshFilter.transform.localPosition);
            }
            
            Gizmos.DrawMesh(meshFilter.sharedMesh, position, rotation, meshFilter.transform.lossyScale);
        }

        void Awake()
        {
            openTween = transform.DOLocalRotate(transform.localEulerAngles + rotationAxis * rotationAmount, rotationTime);
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
