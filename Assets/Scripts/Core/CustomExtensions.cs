using UnityEngine;
using UnityEngine.InputSystem;

namespace Core
{
    public static class CustomExtensions
    {
        public static string FormatDisplayString(this InputAction action, bool withBrackets = true) =>
            FormatDisplayString(action.bindings[0], withBrackets);
        
        public static string FormatDisplayString(this InputBinding binding, bool withBrackets = true)
        {
            var bind = binding.ToDisplayString(InputBinding.DisplayStringOptions.DontIncludeInteractions).ToUpper();
            return withBrackets ? $"[{bind}]" : bind;
        }

        public static bool IsInLayerMask(this GameObject gameObject, LayerMask layerMask) =>
            IsInLayerMask(gameObject.layer, layerMask);
        
        public static bool IsInLayerMask(int layer, LayerMask layerMask) =>
            layerMask == (layerMask | (1 << layer));
    }
}
