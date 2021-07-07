using UnityEngine;

namespace Interactables
{
    public class Hoverable : MonoBehaviour
    {
        public InteractionIcon icon;

        public void OnHover()
        {
            if (!enabled) return;
            IconHandler.Instance.ShowIcon(icon);
        }
        
        public void OnHoverExit()
        {
            if (!enabled) return;
            IconHandler.Instance.HideIcon();
        }
    }
}
