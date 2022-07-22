using UnityEditor;
using UnityEngine;

namespace Products.Editor
{
    [CustomEditor(typeof(ProductInfo))]
    public class ProductInfoEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            var info = (ProductInfo)target;

            var prefab = serializedObject.FindProperty("prefab").objectReferenceValue as GameObject;

            if (!prefab)
            {
                EditorGUILayout.HelpBox("Assign a product prefab.", MessageType.Error);
                return;
            }
            
            if (prefab.TryGetComponent(out ProductIdentifier id) && id.productInfo == info) return;

            EditorGUILayout.HelpBox(
                $"The selected prefab does not have a {nameof(ProductIdentifier)} " +
                $"or does not have the current {nameof(ProductInfo)} selected.",
                MessageType.Warning);

            if (GUILayout.Button("Fix Now"))
            {
                if (!id) id = prefab.AddComponent<ProductIdentifier>();

                id.productInfo = info;
                
                PrefabUtility.SavePrefabAsset(prefab);
            }
        }
    }
}
