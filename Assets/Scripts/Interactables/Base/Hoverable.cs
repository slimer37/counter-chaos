using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Interactables.Base
{
    [DisallowMultipleComponent]
    public class Hoverable : MonoBehaviour
    {
        public InteractionIcon icon;

        public Func<Transform, bool> OnAttemptHover;
        
        IconHandler tempIconHandler;
        Renderer[] renderers;
        bool hoverMaterialIsSet;
        
        readonly Dictionary<Type, object> cachedHandlers = new Dictionary<Type, object>();
        static Material hoveredMaterial;

        [RuntimeInitializeOnLoadMethod]
        static void Init()
        {
            hoveredMaterial = Resources.Load<Material>("Materials/Hovered");
            if (!hoveredMaterial)
                throw new Exception("Couldn't find a hovered material in Resources/Materials. (Looked for Hovered.mat)");
        }

        void Awake() => renderers = GetComponentsInChildren<Renderer>();

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
                SetHoveredMaterial(true);
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
            SetHoveredMaterial(false);
        }

        void SetHoveredMaterial(bool value)
        {
            if (hoverMaterialIsSet == value) return;
            hoverMaterialIsSet = value;
            foreach (var rend in renderers)
            {
                var matList = rend.sharedMaterials.ToList();
                if (value) matList.Add(hoveredMaterial);
                else matList.RemoveAt(matList.Count - 1);
                rend.sharedMaterials = matList.ToArray();
            }
        }
    }
}
