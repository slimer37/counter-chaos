using UnityEngine;

namespace Customization
{
    public class PlayerAppearance : MonoBehaviour
    {
        [SerializeField] Renderer[] skinRenderers;

        void Awake()
        {
            var color = SkinColor.GetPref();

            foreach (var r in skinRenderers)
                r.material.color = color;
        }
    }
}