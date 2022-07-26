using UnityEditor;

namespace Interactables.Holding
{
    [CustomEditor(typeof(Pickuppable))]
    public class PickuppableEditor : Editor
    {
        SerializedProperty infoProp;
        SerializedProperty useColBoundsProp;
        SerializedProperty boxColProp;
        SerializedProperty rbProp;
        SerializedProperty rendProp;
        
        void OnEnable()
        {
            infoProp = serializedObject.FindProperty("<Info>k__BackingField");
            useColBoundsProp = serializedObject.FindProperty("useColliderBounds");
            boxColProp = serializedObject.FindProperty("boxColliders");
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
            
            EditorGUILayout.PropertyField(rbProp);
            EditorGUILayout.PropertyField(rendProp);

            serializedObject.ApplyModifiedProperties();
        }
    }
}