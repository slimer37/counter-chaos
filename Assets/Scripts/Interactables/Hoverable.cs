using System;
using UnityEngine;

namespace Interactables
{
    public class Hoverable : MonoBehaviour
    {
        public InteractionIcon icon;
        
        IconHandler tempIconHandler;

        internal void OnHover(IconHandler iconHandler)
        {
            if (!enabled) return;
            tempIconHandler = iconHandler;
            iconHandler.ShowIcon(icon);
        }
        
        internal void OnHoverExit()
        {
            if (!enabled) return;
            if (!tempIconHandler) throw new InvalidOperationException("HoverExit called without a target.");
            HideIcon();
        }

        void OnDisable()
        {
            if (!tempIconHandler) return;
            HideIcon();
        }

        void HideIcon()
        {
            tempIconHandler.HideIcon();
            tempIconHandler = null;
        }
    }
}
