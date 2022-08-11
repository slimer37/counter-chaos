using UnityEngine;

namespace Interactables.Base
{
    public interface IInteractable
    {
        public InteractionIcon Icon => InteractionIcon.Hand;

        public bool CanInteract(Transform sender) => true;
        public void OnInteract(Transform sender) { }
        
        /// <summary>
        /// Allows a child class to customize the behavior of chain interaction.
        /// This method is usually used to check CanInteract() before calling OnInteract().
        /// </summary>
        /// <param name="sender"></param>
        /// <returns>True if the next Interactable in the chain should receive input, false otherwise.</returns>
        public bool OnProcessInteract(Transform sender)
        {
            if (!CanInteract(sender)) return true;
            OnInteract(sender);
            return false;
        }
        
        public void OnStopInteract(Transform sender) { }
    }

    public interface ISecondaryInteractable
    {
        public void OnSecondaryInteract(Transform sender) { }
        public void OnStopSecondaryInteract(Transform sender) { }
    }
}
