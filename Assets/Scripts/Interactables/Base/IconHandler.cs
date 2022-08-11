using UnityEngine;
using UnityEngine.UI;

namespace Interactables.Base
{
    public enum InteractionIcon { Invalid, Hand, Access, Pickup, Pull, Push, Eye, Door }
    
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

        internal void TempSetAlpha(float a)
        {
            var col = Color.white;
            col.a = a;
            iconImage.color = pointerImage.color = col;
        }

        void EnableIcon(bool value)
        {
            if (iconImage.enabled == value) return;
            TempSetAlpha(1);
            // The icon and pointer are always in opposite states.
            iconImage.enabled = value;
            pointerImage.enabled = !value;
        }
    }
}
