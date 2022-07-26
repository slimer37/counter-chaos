using UnityEditor;
using UnityEngine;

namespace Interactables.Holding
{
    [CustomPropertyDrawer(typeof(HoldableInfo))]
    public class HoldableInfoDrawer : PropertyDrawer
    {
        bool overridePos;
        bool overrideRot;
        bool expandOverrides;
        
        bool foldout;

        void PropertyField(SerializedProperty property, string name) =>
            EditorGUILayout.PropertyField(property.FindPropertyRelative(name));
        
        public override void OnGUI(Rect position, SerializedProperty property,
            GUIContent label)
        {
            foldout = EditorGUILayout.BeginFoldoutHeaderGroup(foldout, label);
            EditorGUILayout.EndFoldoutHeaderGroup();

            if (!foldout) return;
            
            PropertyField(property, "label");
            PropertyField(property, "canBeDropped");
            PropertyField(property, "canBeThrown");
            PropertyField(property, "canBeHung");
            PropertyField(property, "groundPlacementOnly");

            var holdPosProp = property.FindPropertyRelative("overridePosition");
            var zeroRotProp = property.FindPropertyRelative("useRotationIfZero");
            var holdRotProp = property.FindPropertyRelative("overrideRotation");

            overridePos = holdPosProp.vector3Value != Vector3.zero;
            overrideRot = holdRotProp.vector3Value != Vector3.zero || zeroRotProp.boolValue;
            var anyOverride = overridePos || overrideRot;

            var overrides = anyOverride switch
            {
                true when overridePos && overrideRot => "position and rotation",
                true when overridePos => "position",
                true when overrideRot => "rotation",
                _ => "none"
            };
                
            expandOverrides =
                EditorGUILayout.BeginFoldoutHeaderGroup(expandOverrides, $"Overrides ({overrides} active)");

            if (expandOverrides)
            {
                EditorGUILayout.PropertyField(holdPosProp);

                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(zeroRotProp);
                EditorGUILayout.PropertyField(holdRotProp);

                if (anyOverride && GUILayout.Button("Clear Overrides"))
                {
                    zeroRotProp.boolValue = false;
                    holdPosProp.vector3Value = Vector3.zero;
                    holdRotProp.vector3Value = Vector3.zero;
                }
            }
                
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
    }
}