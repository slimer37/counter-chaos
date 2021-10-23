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
    }
}
