using UnityEditor;
using UnityEngine;

namespace Products
{
    [CustomEditor(typeof(ProductInfo))]
    public class ProductInfoEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            var info = (ProductInfo)target;

            var prefab = info.PrefabEditorAsset;

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
