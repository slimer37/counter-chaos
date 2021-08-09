using System;
using System.Collections.Generic;
using Plugins;
using UnityEngine;

namespace Interactables.Base
{
    [DisallowMultipleComponent]
    public class Hoverable : MonoBehaviour
    {
        public InteractionIcon icon;

        public Func<Transform, bool> OnAttemptHover;
        
        IconHandler tempIconHandler;
        Outline outline;
        
        readonly Dictionary<Type, object> cachedHandlers = new Dictionary<Type, object>();

        float startHoverTime;
        
        const float FlashSpeed = 1.5f;

        void Awake()
        {
            outline = gameObject.AddComponent<Outline>();
            outline.OutlineColor = Color.yellow;
            outline.OutlineMode = Outline.Mode.OutlineVisible;
            outline.OutlineWidth = 10;
            outline.enabled = false;
        }

        public THandler[] GetOnce<THandler>()
        {
            var handler = typeof(THandler);
            
            if (!cachedHandlers.ContainsKey(handler))
                cachedHandlers[handler] = GetComponents<THandler>();

            return (THandler[])cachedHandlers[handler];
        }

        void OnValidate()
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
                
                if (!outline.enabled)
                {
                    startHoverTime = Time.time;
                    SetOutlineAlpha(0);
                    outline.enabled = true;
                }
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
            outline.enabled = false;
        }

        void SetOutlineAlpha(float a)
        {
            var color = outline.OutlineColor;
            color.a = a;
            outline.OutlineColor = color;
        }

        void Update()
        {
            if (!outline.enabled) return;

            var t = (Time.time - startHoverTime) * FlashSpeed;
            var a = -Mathf.Cos(t * 2 * Mathf.PI) / 2 + 0.5f;
            SetOutlineAlpha(a);
        }
    }
}
