using UnityEngine;

namespace Interactables.Base
{
    public interface IInteractable
    {
        public InteractionIcon Icon => InteractionIcon.Access;
        public bool PassThrough => false;

        public bool CanInteract(Transform sender) => true;
        
        public void OnInteract(Transform sender) { }
        public void OnStopInteract(Transform sender) { }
    }

    public interface ISecondaryInteractable
    {
        public void OnSecondaryInteract(Transform sender) { }
        public void OnStopSecondaryInteract(Transform sender) { }
    }
}
