using System;
using UnityEngine;

namespace Interactables
{
    public class Hoverable : MonoBehaviour
    {
        public InteractionIcon icon;

        public Func<Transform, bool> OnAttemptHover;
        
        IconHandler tempIconHandler;

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
