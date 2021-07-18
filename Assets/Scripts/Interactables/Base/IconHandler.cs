using UnityEngine;
using UnityEngine.UI;

namespace Interactables.Base
{
    public enum InteractionIcon { Invalid, Access, }
    
    public class IconHandler : MonoBehaviour
    {
        [SerializeField] Image iconImage;
        [SerializeField] Image pointerImage;
        [SerializeField] Sprite[] icons;

        internal void ShowIcon(InteractionIcon icon)
        {
            EnableIcon(true);
            iconImage.sprite = icons[(int)icon];
        }

        internal void HideIcon() => EnableIcon(false);

        void EnableIcon(bool value)
        {
            if (iconImage.enabled == value) return;
            // The icon and pointer are always in opposite states.
            iconImage.enabled = value;
            pointerImage.enabled = !value;
        }
    }
}
