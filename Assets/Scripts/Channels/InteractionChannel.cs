using System;
using Core;
using UnityEngine;

namespace Interactables.Base
{
    // with help from https://www.youtube.com/watch?v=WLDgtRNK2VE
    [CreateAssetMenu(menuName = "Events/Interaction Channel")]
    public class InteractionChannel : ScriptableObject
    {
        // Interaction channel shows an active state when any hold action (i.e. pulling a door or attaching a shelf)
        // is in progress, allowing any systems, primarily player interaction, to halt cleanly.
        public bool IsHoldingInteraction { get; private set; }
        public event Action<bool> OnActivate;
        public event Action OnDeactivate;
        
        public event Action OnInteractHold;
        public event Action OnInteractRelease;

        Controls controls;

        void OnEnable()
        {
            if (controls == null)
            {
                controls = new Controls();
                controls.Gameplay.Interact.performed += _ => OnInteractHold?.Invoke();
                controls.Gameplay.Interact.canceled += _ => OnInteractRelease?.Invoke();
            }
            
            controls.Enable();
        }

        void OnDisable() => controls.Disable();

        public void Activate(bool exitHover = false)
        {
            if (IsHoldingInteraction)
                Debug.LogWarning("Calling activate while already active. This is probably unintended.");
            
            IsHoldingInteraction = true;
            OnActivate?.Invoke(exitHover);
        }

        public void Deactivate()
        {
            if (!IsHoldingInteraction)
                Debug.LogWarning("Calling deactivate while already inactive. This is probably unintended.");
            
            IsHoldingInteraction = false;
            OnDeactivate?.Invoke();
        }
    }
}