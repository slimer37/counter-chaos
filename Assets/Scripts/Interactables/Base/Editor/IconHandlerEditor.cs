using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Interactables.Base
{
    [CustomEditor(typeof(IconHandler))]
    public class IconHandlerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            // Grab assigned sprites
            
            var iconArray = serializedObject.FindProperty("icons");
            var icons = new List<Sprite>();
            
            if (iconArray.arraySize > 0)
                for (var i = 0; i < iconArray.arraySize; i++)
                {
                    var value = iconArray.GetArrayElementAtIndex(i).objectReferenceValue;
                    icons.Add((Sprite)value);
                }
            
            // Automatically match array length
            
            var iconEnumNames = System.Enum.GetNames(typeof(InteractionIcon));
            var expectedNumIcons = iconEnumNames.Length;
            if (expectedNumIcons != icons.Count)
            {
                for (var i = 0; i < Mathf.Abs(expectedNumIcons - icons.Count); i++)
                {
                    if (icons.Count < expectedNumIcons)
                    {
                        iconArray.InsertArrayElementAtIndex(iconArray.arraySize - 1);
                        iconArray.GetArrayElementAtIndex(iconArray.arraySize - 1).objectReferenceValue = null;
                        icons.Add(null);
                    }
                    else
                    {
                        iconArray.DeleteArrayElementAtIndex(iconArray.arraySize - 1);
                        icons.RemoveAt(icons.Count - 1);
                    }
                }

                serializedObject.ApplyModifiedProperties();
            }
            
            // Check and gather wrong names

            var wrongNames = "Some icons are mismatched:";
            var anyNamesWrong = false;
            
            for (var i = 0; i < Mathf.Min(icons.Count, iconEnumNames.Length); i++)
            {
                var expectedName = iconEnumNames[i];
                if ((icons[i]?.name ?? "") != expectedName)
                {
                    anyNamesWrong = true;
                    wrongNames += $"\nIcon '{icons[i]?.name ?? "null"}' at index {i} is incorrect. " +
                                  $"(Expected '{expectedName}')";
                }
            }

            if (!anyNamesWrong) return;
            EditorGUILayout.HelpBox(wrongNames, MessageType.Error);
        }
    }
}
