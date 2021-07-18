using UnityEngine;

namespace Interactables
{
    public interface IInteractHandler
    {
        public void OnInteract(Transform sender);
    }

    public interface IStopInteractHandler
    {
        public void OnStopInteract(Transform sender);
    }
}
