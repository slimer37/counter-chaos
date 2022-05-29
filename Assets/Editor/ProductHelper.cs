using System;
using Checkout;
using Interactables.Base;
using Interactables.Holding;
using Products;
using UnityEditor;
using UnityEngine;

namespace Project.Editor
{
    public class ProductHelper : EditorWindow
    {
        enum ColliderShape { Box, Mesh, Sphere}
        
        string productInfoFolder = "Assets/Product Assets/";
        string ProductAssetPath => productInfoFolder + productName + ".asset";
        
        string productName;
        string productDisplayName;
        float price;
        string productDescription;
        
        ColliderShape colliderShape;
        
        bool createNewProductInfo = true;
        bool foldout = true;
        
        [MenuItem("Tools/Product Helper")]
        static void ShowWindow() => GetWindow<ProductHelper>("Product Helper");

        void OnGUI()
        {
            var selected = Selection.activeGameObject;
            
            createNewProductInfo = EditorGUILayout.Toggle("Create New Product Asset", createNewProductInfo);
            if (createNewProductInfo)
            {
                foldout = EditorGUILayout.BeginFoldoutHeaderGroup(foldout, "Product Asset");
                if (foldout)
                {
                    productName = EditorGUILayout.TextField("File Name", productName);
                    
                    if (AssetDatabase.LoadAssetAtPath<ProductInfo>(ProductAssetPath))
                        EditorGUILayout.HelpBox("File already exists and will be overridden.", MessageType.Warning);
                    
                    productDisplayName = EditorGUILayout.TextField("Display Name", productDisplayName);
                    price = EditorGUILayout.FloatField("Price", price);
                    EditorGUILayout.PrefixLabel("Description");
                    productDescription =
                        EditorGUILayout.TextArea(productDescription, GUILayout.Height(EditorStyles.textArea.lineHeight * 3));
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
            }

            if (!selected)
            {
                if (createNewProductInfo && GUILayout.Button("Create Product Asset"))
                    CreateProductInfo();
                
                EditorGUILayout.HelpBox("No GameObject selected.", MessageType.Info);
                return;
            }
            
            colliderShape = (ColliderShape)EditorGUILayout.EnumPopup("Collider Type", colliderShape);
            
            if (GUILayout.Button($"Make {selected.name} into new product"))
            {
                var info = createNewProductInfo ? CreateProductInfo() : null;
                MakeIntoProduct(selected, info, colliderShape);
            }
        }

        ProductInfo CreateProductInfo()
        {
            var info = AssetDatabase.LoadAssetAtPath<ProductInfo>(ProductAssetPath);
            if (!info)
            {
                info = CreateInstance<ProductInfo>();
                AssetDatabase.CreateAsset(info, ProductAssetPath);
            }
            var so = new SerializedObject(info);
            so.FindProperty("<DisplayName>k__BackingField").stringValue = productDisplayName;
            so.FindProperty("<Price>k__BackingField").floatValue = price;
            so.FindProperty("<Description>k__BackingField").stringValue = productDescription;
            so.ApplyModifiedProperties();
            return info;
        }

        void OnSelectionChange() => Repaint();

        static void MakeIntoProduct(GameObject gameObject, ProductInfo info, ColliderShape shape)
        {
            gameObject.layer = LayerMask.NameToLayer("Interactable");
            gameObject.tag = "Product";
            
            var col = shape switch
            {
                ColliderShape.Box => typeof(BoxCollider),
                ColliderShape.Mesh => typeof(MeshCollider),
                ColliderShape.Sphere => typeof(SphereCollider),
                _ => throw new ArgumentException("Invalid collider shape.", nameof(shape))
            };
            if (!gameObject.GetComponent(col)) gameObject.AddComponent(col);

            var identifier = new SerializedObject(AddIfNeeded<ProductIdentifier>());
            identifier.FindProperty("productInfo").objectReferenceValue = info;
            
            var hoverable = new SerializedObject(AddIfNeeded<Hoverable>());
            hoverable.FindProperty("icon").intValue = 1;

            identifier.ApplyModifiedProperties();
            hoverable.ApplyModifiedProperties();
            
            AddIfNeeded<Rigidbody>();
            
            var pbSo = new SerializedObject(AddIfNeeded<Pickuppable>());
            pbSo.FindProperty("<Info>k__BackingField").FindPropertyRelative("label").stringValue = info.DisplayName;
            pbSo.ApplyModifiedProperties();

            TComponent AddIfNeeded<TComponent>() where TComponent : Component
            {
                var component = gameObject.GetComponent<TComponent>();
                if (!component)
                    component = gameObject.AddComponent<TComponent>();
                return component;
            }
        }
    }
}
