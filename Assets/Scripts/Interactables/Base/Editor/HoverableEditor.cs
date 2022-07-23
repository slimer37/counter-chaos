using UnityEditor;
using UnityEngine;

namespace Interactables.Base
{
    [CustomEditor(typeof(Hoverable))]
    public class HoverableEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            var interactables = Selection.activeGameObject.GetComponents<IInteractable>();
            var labels = new string[interactables.Length];
            for (var i = 0; i < interactables.Length; i++)
            {
                labels[i] = (interactables[i] as MonoBehaviour)?.GetType().Name;
                if (interactables[i].PassThrough)
                    labels[i] += " - pass-through";
            }

            EditorGUILayout.HelpBox("Current order:\n" + string.Join("\n", labels), MessageType.None);
        }
    }
}