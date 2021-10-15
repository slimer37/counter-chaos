using UnityEditor;
using UnityEngine;

namespace Interactables.Base.Editor
{
    public static class VerifyHoverables
    {
        [InitializeOnLoadMethod]
        static void CheckForHoverables()
        {
            foreach (var go in Object.FindObjectsOfType<GameObject>())
                if (go.TryGetComponent<IInteractHandler>(out _) && !go.TryGetComponent<Hoverable>(out _))
                    Debug.LogError($"{go.name} is missing a Hoverable component.");
        }
    }
}
