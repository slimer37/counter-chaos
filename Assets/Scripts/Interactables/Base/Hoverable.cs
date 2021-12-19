using System;
using System.Collections.Generic;
using UnityEngine;

namespace Interactables.Base
{
    [DisallowMultipleComponent]
    public class Hoverable : MonoBehaviour
    {
        public InteractionIcon icon = InteractionIcon.Access;

        [SerializeField] bool useOutline = true;

        public event Func<Transform, bool> OnAttemptHover
        {
            add => RegisterPriorityCheck(value, 0);
            remove => hoverChecks.Remove(value);
        }

        readonly List<Func<Transform, bool>> hoverChecks = new();
        
        IconHandler tempIconHandler;
        Highlight highlight;
        
        readonly Dictionary<Type, object> cachedHandlers = new();

        public bool LastCheckSuccessful { get; private set; }

        int highestCallbackPriority;

        void Awake()
        {
            if (!useOutline) return;
            highlight = gameObject.AddComponent<Highlight>();
            highlight.enabled = false;
        }

        public void ClearUnderPriority(int priority)
        {
            if (highestCallbackPriority >= priority) return;
            highestCallbackPriority = priority;
            hoverChecks.Clear();
        }

        public void RegisterPriorityCheck(Func<Transform, bool> callback, int priority)
        {
            ClearUnderPriority(priority);
            
            // If previous priority was less or equal, it is set equal while clearing.
            if (highestCallbackPriority == priority)
                hoverChecks.Add(callback);
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

        bool CanHover(Transform sender)
        {
            foreach (var check in hoverChecks)
            { if (!check(sender)) return false; }
            return true;
        }

        public void OnHover(IconHandler iconHandler, Transform sender)
        {
            if (!enabled) return;

            LastCheckSuccessful = CanHover(sender);
            if (LastCheckSuccessful)
            {
                tempIconHandler = iconHandler;
                iconHandler.ShowIcon(icon);

                if (!useOutline || highlight.enabled) return;
                highlight.enabled = true;
            }
            else
                HideIcon();
        }
        
        public void OnHoverExit()
        {
            if (!enabled) return;
            HideIcon();
        }

        public void SetIconAlpha(float a) => tempIconHandler?.TempSetAlpha(a);

        void OnDisable() => HideIcon();

        void HideIcon()
        {
            if (tempIconHandler)
            {
                tempIconHandler.HideIcon();
                tempIconHandler = null;
            }

            if (useOutline)
                highlight.enabled = false;
        }
    }
}
