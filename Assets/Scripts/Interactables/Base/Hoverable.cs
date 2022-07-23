using System.Linq;
using UnityEngine;

namespace Interactables.Base
{
    [DisallowMultipleComponent]
    public class Hoverable : MonoBehaviour
    {
        [SerializeField] bool useOutline = true;
        
        IInteractable[] interactables;
        IconHandler tempIconHandler;
        Highlight highlight;

        void Awake()
        {
            interactables = GetComponents<IInteractable>();
            
            if (!useOutline) return;
            highlight = gameObject.AddComponent<Highlight>();
            highlight.enabled = false;
        }

        void OnValidate()
        {
            if (gameObject.layer != LayerMask.NameToLayer("Interactable"))
                Debug.LogWarning($"{name} is not on the Interactable layer.", gameObject);
        }

        public bool TryGetInteractable(Transform sender, out IInteractable interactable)
        {
            interactable = null;
            
            foreach (var i in interactables)
            {
                if (i.CanInteract(sender))
                {
                    interactable = i;
                    return true;
                }
            }

            return false;
        }

        public void OnHover(IconHandler iconHandler, Transform sender)
        {
            if (TryGetInteractable(sender, out var interactable))
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
