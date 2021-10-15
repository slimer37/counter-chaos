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

        [SerializeField] bool useOutline = true;

        public event Func<Transform, bool> OnAttemptHover
        {
            add => hoverChecks.Add(value);
            remove => hoverChecks.Remove(value);
        }

        readonly List<Func<Transform, bool>> hoverChecks = new List<Func<Transform, bool>>();
        
        IconHandler tempIconHandler;
        Outline outline;
        
        readonly Dictionary<Type, object> cachedHandlers = new Dictionary<Type, object>();

        float startHoverTime;
        
        const float FlashSpeed = 1.5f;

        public bool LastCheckSuccessful { get; private set; }
        
        // No direct disabling!
        new public bool enabled => base.enabled;

        int highestCallbackPriority;

        void Awake()
        {
            if (!useOutline) return;
            outline = gameObject.AddComponent<Outline>();
            outline.OutlineColor = Color.yellow;
            outline.OutlineMode = Outline.Mode.OutlineVisible;
            outline.OutlineWidth = 10;
            outline.enabled = false;
        }

        public void ClearUnderPriority(int priority)
        {
            if (highestCallbackPriority >= priority) return;
            highestCallbackPriority = priority;
            hoverChecks.Clear();
        }

        public void RegisterPriorityCheck(Func<Transform, bool> callback, int priority = 0)
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

                if (!useOutline || outline.enabled) return;
                startHoverTime = Time.time;
                SetOutlineAlpha(0);
                outline.enabled = true;
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

            if (!useOutline) return;
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
            if (!useOutline || !outline.enabled) return;

            var t = (Time.time - startHoverTime) * FlashSpeed;
            var a = -Mathf.Cos(t * 2 * Mathf.PI) / 2 + 0.5f;
            SetOutlineAlpha(a);
        }
    }
}
