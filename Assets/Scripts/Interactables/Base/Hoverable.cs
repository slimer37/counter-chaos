using System;
using System.Collections.Generic;
using UnityEngine;

namespace Interactables.Base
{
    [DisallowMultipleComponent]
    public class Hoverable : MonoBehaviour
    {
        public InteractionIcon icon;

        public Func<Transform, bool> OnAttemptHover;
        
        IconHandler tempIconHandler;
        
        readonly Dictionary<Type, object> cachedHandlers = new Dictionary<Type, object>();

        public THandler[] GetOnce<THandler>()
        {
            var handler = typeof(THandler);
            
            if (!cachedHandlers.ContainsKey(handler))
                cachedHandlers[handler] = GetComponents<THandler>();

            return (THandler[])cachedHandlers[handler];
        }

        void Reset()
        {
            if (gameObject.layer != LayerMask.NameToLayer("Interactable"))
                Debug.LogWarning($"{name} is not on the Interactable layer.", gameObject);
        }

        public void OnHover(IconHandler iconHandler, Transform sender)
        {
            if (!enabled) return;
            
            if (OnAttemptHover == null || OnAttemptHover(sender))
            {
                tempIconHandler = iconHandler;
                iconHandler.ShowIcon(icon);
            }
            else
                HideIcon();
        }
        
        public void OnHoverExit()
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
