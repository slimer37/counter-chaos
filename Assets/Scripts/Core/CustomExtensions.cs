using UnityEngine;
using UnityEngine.InputSystem;

namespace Core
{
    public static class CustomExtensions
    {
        public static bool IsInLayerMask(this GameObject gameObject, LayerMask layerMask) =>
            IsInLayerMask(gameObject.layer, layerMask);
        
        public static bool IsInLayerMask(int layer, LayerMask layerMask) =>
            layerMask == (layerMask | (1 << layer));
    }
}
