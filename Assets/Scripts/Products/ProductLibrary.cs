using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Products
{
    public static class ProductLibrary
    {
        static ProductInfo[] allProducts;
        static readonly List<ProductIdentifier> productInstances = new();
        
        public static IReadOnlyList<ProductInfo> AllProducts => allProducts;

        const string ProductsKey = "product";

        [RuntimeInitializeOnLoadMethod]
        static void Init()
        {
            // Clear instances each scene
            SceneManager.sceneLoaded += (_, m) => {
                if (m == LoadSceneMode.Single) productInstances.Clear();
            };

            var i = 0;
            var productLoadHandle = Addressables.LoadAssetsAsync<ProductInfo>(ProductsKey, info => {
                info.Init(i);
                i++;
            });
            
            allProducts = productLoadHandle.WaitForCompletion().ToArray();
        }

        public static void AddInstance(ProductIdentifier productIdentifier) => productInstances.Add(productIdentifier);
        public static void RemoveInstance(ProductIdentifier productIdentifier) => productInstances.Remove(productIdentifier);

        public static ProductIdentifier GetRandomProductInstance() => productInstances[Random.Range(0, productInstances.Count)];

        public static bool TryGetProductInfo(Transform transform, out ProductInfo info) =>
            info = productInstances.Find(identifier => identifier.transform == transform).productInfo;
    }
}
