using System.Linq;
using UnityEngine;

namespace Customization
{
    public class SkinColor : MonoBehaviour
    {
        [SerializeField] Renderer[] exclude;
        
        Renderer[] bodyRenderers;

        void Awake()
        {
            if (bodyRenderers != null) return;
            
            bodyRenderers = GetComponentsInChildren<Renderer>();
            
            var list = bodyRenderers.ToList();
            foreach (var r in exclude)
                list.Remove(r);
            
            bodyRenderers = list.ToArray();
        }

        public void SetColor(Color color)
        {
            if (bodyRenderers == null) Awake();
            
            foreach (var rend in bodyRenderers)
                rend.material.color = color;
        }
    }
}
