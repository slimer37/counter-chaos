using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Interactables.Base
{
    [CustomEditor(typeof(Hoverable))]
    public class HoverableEditor : Editor
    {
        object GetField(string field)
        {
            if (typeof(Hoverable).GetField(field, BindingFlags.NonPublic | BindingFlags.Instance) is not { } info)
                throw new ArgumentException($"No field found with name '{field}'", nameof(field));
            return info.GetValue(target);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var go = Selection.activeGameObject;
            var playing = EditorApplication.isPlaying;
            
            var interactables = playing
                ? ((object[])GetField("interactables"))?.Cast<IInteractable>().ToArray()
                : go.GetComponents<IInteractable>();
            var labels = new string[interactables?.Length ?? 0];
            
            if (interactables == null || interactables.Length == 0)
                labels = new[] { "None" };
            else
            {
                for (var i = 0; i < interactables.Length; i++)
                {
                    labels[i] = interactables[i].GetType().Name;
                }
            }

            var secondaries = go.GetComponents<ISecondaryInteractable>();
            var secondary = "None";

            if (playing)
            {
                var c = (ISecondaryInteractable)GetField("secondaryInteractable");
                if (c != null) secondary = c.GetType().Name;
            }
            else if (secondaries.Length > 0)
                secondary = secondaries[0].GetType().Name;
            
            if (secondaries.Length > 1)
                EditorGUILayout.HelpBox(
                    $"There is more than one {nameof(ISecondaryInteractable)} component. " +
                    $"Each component after {secondaries[0].GetType().Name} will be ignored.",
                    MessageType.Error);

            var style = new GUIStyle(EditorStyles.helpBox);
            style.fontSize += 2;
            style.stretchHeight = true;
            style.richText = true;
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Primary", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(string.Join("\n", labels), style);
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Secondary", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(secondary, style);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            
            if (playing)
                EditorGUILayout.HelpBox(
                    "The order above will not respond to changes in the Inspector in play mode.",
                    MessageType.Info);
        }
    }
}