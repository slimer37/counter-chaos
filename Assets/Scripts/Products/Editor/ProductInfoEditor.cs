using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Products.Editor
{
    [CustomEditor(typeof(ProductInfo))]
    public class ProductInfoEditor : UnityEditor.Editor
    {
        static AddressableAssetGroup productGroup;
        
        [InitializeOnLoadMethod]
        static void Init()
        {
            productGroup = AddressableAssetSettingsDefaultObject.Settings.FindGroup("Products");
        }
        
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

            if (!prefab.TryGetComponent(out ProductIdentifier id) || id.productInfo != info)
            {
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
            
            if (!IsAssetAddressable(info))
            {
                EditorGUILayout.HelpBox("This product is not properly set as an addressable.",
                    MessageType.Error);

                if (GUILayout.Button("Modify Addressable"))
                    AddAssetToAddressables(info);
            }
        }
        
        static void AddAssetToAddressables(ProductInfo info)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var assetPath = AssetDatabase.GetAssetPath(info);
            var assetGuid = AssetDatabase.AssetPathToGUID(assetPath);
            var entry = settings.CreateOrMoveEntry(assetGuid, productGroup);
            entry.SetAddress(info.DisplayName);
        }
        
        static bool IsAssetAddressable(ProductInfo info)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var entry = settings.FindAssetEntry(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(info)));
            return entry != null
                   && entry.parentGroup == productGroup
                   && entry.address == info.DisplayName;
        }
    }
}
