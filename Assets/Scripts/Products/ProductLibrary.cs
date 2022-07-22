using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Products
{
    public static class ProductLibrary
    {
        static ProductInfo[] allProducts;
        static readonly List<ProductIdentifier> ProductInstances = new();
        
        public static IReadOnlyList<ProductInfo> AllProducts => allProducts;
        public static event Action AssetsFullyLoaded;
        
        public static bool AssetsAreFullyLoaded { get; private set; }

        const string ProductsKey = "product";

        [RuntimeInitializeOnLoadMethod]
        static void Init()
        {
            // Clear instances each scene
            SceneManager.sceneLoaded += (_, m) => {
                if (m == LoadSceneMode.Single) ProductInstances.Clear();
            };

            var i = 0;
            var productLoadHandle = Addressables.LoadAssetsAsync<ProductInfo>(ProductsKey, info => {
                info.Init(i);
                i++;
            });
            
            allProducts = productLoadHandle.WaitForCompletion().ToArray();
            productLoadHandle.Completed += _ => {
                AssetsAreFullyLoaded = true;
                AssetsFullyLoaded?.Invoke();
            };
        }

        public static void AddInstance(ProductIdentifier productIdentifier) => ProductInstances.Add(productIdentifier);
        public static void RemoveInstance(ProductIdentifier productIdentifier) => ProductInstances.Remove(productIdentifier);

        public static ProductIdentifier GetRandomProductInstance() => ProductInstances[Random.Range(0, ProductInstances.Count)];

        public static bool TryGetProductInfo(Transform transform, out ProductInfo info) =>
            info = ProductInstances.Find(identifier => identifier.transform == transform).productInfo;
    }
}
