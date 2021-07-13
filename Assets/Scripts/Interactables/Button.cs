using UnityEngine;
using UnityEngine.Events;

namespace Interactables
{
    public class Button : MonoBehaviour, IInteractable, IStopInteractHandler
    {
        public UnityEvent onPress;
        public UnityEvent onRelease;

        public void OnInteract(Transform sender) => onPress.Invoke();
        public void OnStopInteract(Transform sender) => onRelease.Invoke();
    }
}
