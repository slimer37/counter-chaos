using System;
using UnityEngine;

namespace Interactables
{
    public class Hoverable : MonoBehaviour
    {
        public InteractionIcon icon;

        public Func<Transform, bool> OnAttemptHover;
        
        IconHandler tempIconHandler;

        void Reset()
        {
            if (gameObject.layer != LayerMask.NameToLayer("Interactable"))
                Debug.Log($"{name} is not on the Interactable layer.");
        }

        internal void OnHover(IconHandler iconHandler, Transform sender)
        {
            if (!enabled) return;
            if (OnAttemptHover != null && !OnAttemptHover(sender)) return;
            
            tempIconHandler = iconHandler;
            iconHandler.ShowIcon(icon);
        }
        
        internal void OnHoverExit()
        {
            if (!enabled) return;
            HideIcon();
        }

        void OnDisable() => HideIcon();

        void HideIcon()
        {
            if (!tempIconHandler) return;
            tempIconHandler.HideIcon();
            tempIconHandler = null;
        }
    }
}
