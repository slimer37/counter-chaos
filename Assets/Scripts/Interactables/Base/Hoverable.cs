using System;
using UnityEngine;

namespace Interactables.Base
{
    [DisallowMultipleComponent]
    public class Hoverable : MonoBehaviour
    {
        [SerializeField] bool useOutline = true;
        
        IInteractable[] interactables;
        ISecondaryInteractable secondaryInteractable;
        IconHandler tempIconHandler;
        Highlight highlight;

        void Awake()
        {
            interactables = GetComponents<IInteractable>();
            secondaryInteractable = GetComponent<ISecondaryInteractable>();
            
            if (!useOutline) return;
            highlight = gameObject.AddComponent<Highlight>();
            highlight.enabled = false;
        }

        void OnValidate()
        {
            if (gameObject.layer != LayerMask.NameToLayer("Interactable"))
                Debug.LogWarning($"{name} is not on the Interactable layer.", gameObject);
        }

        public bool IsInteractable(Transform sender, out IInteractable i)
        {
            IInteractable temp = null;

            var result = ForAllInteractable(sender, i => temp = i, false);

            i = temp;

            return result;
        }

        public void Interact(Transform sender, bool start, bool secondary)
        {
            if (secondary)
            {
                if (secondaryInteractable == null) return;
                if (start) secondaryInteractable.OnSecondaryInteract(sender);
                else secondaryInteractable.OnStopSecondaryInteract(sender);
                return;
            }
            
            ForAllInteractable(sender,
                start
                    ? i => i.OnInteract(sender)
                    : i => i.OnStopInteract(sender),
                true);
        }

        bool ForAllInteractable(Transform sender, Action<IInteractable> action, bool allowPassThrough)
        {
            foreach (var i in interactables)
            {
                if (i.CanInteract(sender))
                {
                    action(i);
                    if (allowPassThrough && i.PassThrough) continue;
                    return true;
                }
            }

            return false;
        }

        public void OnHover(IconHandler iconHandler, Transform sender)
        {
            if (IsInteractable(sender, out var interactable))
            {
                tempIconHandler = iconHandler;
                iconHandler.ShowIcon(interactable.Icon);

                if (!useOutline || highlight.enabled) return;
                highlight.enabled = true;
            }
            else
                OnHoverExit();
        }
        
        public void OnHoverExit()
        {
            if (tempIconHandler)
            {
                tempIconHandler.HideIcon();
                tempIconHandler = null;
            }

            if (useOutline)
                highlight.enabled = false;
        }

        public void SetIconAlpha(float a) => tempIconHandler?.TempSetAlpha(a);

        void OnDisable() => OnHoverExit();
    }
}
