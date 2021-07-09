using UnityEngine;

namespace Interactables
{
    public class Hoverable : MonoBehaviour
    {
        public InteractionIcon icon;
        
        IconHandler tempIconHandler;

        public void OnHover(IconHandler iconHandler)
        {
            if (!enabled) return;
            tempIconHandler = iconHandler;
            iconHandler.ShowIcon(icon);
        }
        
        public void OnHoverExit()
        {
            if (!enabled) return;
            tempIconHandler.HideIcon();
        }
    }
}
