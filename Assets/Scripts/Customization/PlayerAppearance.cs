using UnityEngine;

namespace Customization
{
    public class PlayerAppearance : MonoBehaviour
    {
        [SerializeField] Renderer[] skinRenderers;

        void Start()
        {
            var color = SkinColor.GetPref();

            if (color == null) return;

            foreach (var r in skinRenderers)
                r.material.color = (Color)color;
        }
    }
}