using UnityEditor;
using UnityEngine;

namespace Interactables.Holding
{
    [CustomEditor(typeof(Pickuppable))]
    public class PickuppableEditor : Editor
    {
        SerializedProperty infoProp;
        SerializedProperty useColBoundsProp;
        SerializedProperty boxColProp;
        SerializedProperty holdPosProp;
        SerializedProperty zeroRotProp;
        SerializedProperty holdRotProp;
        SerializedProperty rbProp;
        SerializedProperty rendProp;

        bool overridePos;
        bool overrideRot;
        bool expandOverrides;
        
        void OnEnable()
        {
            infoProp = serializedObject.FindProperty("<Info>k__BackingField");
            useColBoundsProp = serializedObject.FindProperty("useColliderBounds");
            boxColProp = serializedObject.FindProperty("boxColliders");
            holdPosProp = serializedObject.FindProperty("overrideHoldingPosition");
            zeroRotProp = serializedObject.FindProperty("useRotationIfZeroes");
            holdRotProp = serializedObject.FindProperty("overrideHoldingRotation");
            rbProp = serializedObject.FindProperty("rb");
            rendProp = serializedObject.FindProperty("rend");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(infoProp);
            
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Bounds", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(useColBoundsProp);
            
            if (useColBoundsProp.boolValue)
                EditorGUILayout.PropertyField(boxColProp);
            
            EditorGUILayout.Space();
            
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

            expandOverrides = EditorGUILayout.BeginFoldoutHeaderGroup(expandOverrides,
                $"Overrides ({overrides} active)");
            
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
            
            EditorGUILayout.Space();
            
            EditorGUILayout.PropertyField(rbProp);
            EditorGUILayout.PropertyField(rendProp);

            serializedObject.ApplyModifiedProperties();
        }
    }
}