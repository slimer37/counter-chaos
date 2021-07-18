using UnityEngine;

namespace Interactables.Base
{
    public interface IInteractHandler
    {
        public void OnInteract(Transform sender);
    }

    public interface IStopInteractHandler
    {
        public void OnStopInteract(Transform sender);
    }

    public interface ISecondaryInteractHandler
    {
        public void OnSecondaryInteract(Transform sender);
    }

    public interface IStopSecondaryInteractHandler
    {
        public void OnStopSecondaryInteract(Transform sender);
    }
}
