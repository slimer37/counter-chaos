using System.Collections.Generic;
using UnityEngine;

namespace Products
{
    public class ProductManager : MonoBehaviour
    {
        [SerializeField] ProductInfo[] allProducts;

        static List<ProductIdentifier> productInstances;
        static ProductInfo[] currentProductCollection;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 0) return;
            productInstances = new List<ProductIdentifier>();
            currentProductCollection = FindObjectOfType<ProductManager>().allProducts;
        }

        public static void RegisterProduct(ProductIdentifier productIdentifier)
        {
#if UNITY_EDITOR
            var info = productIdentifier.productInfo;
            if (!System.Linq.Enumerable.Contains(currentProductCollection, info))
                Debug.LogWarning($"{info.name} ({info.DisplayName}) is not in the product manager.");
#endif
            productInstances.Add(productIdentifier);
        }
        public static void DeregisterProduct(ProductIdentifier productIdentifier) => productInstances.Remove(productIdentifier);

        public static ProductIdentifier GetRandomProductInstance() => productInstances[Random.Range(0, productInstances.Count)];

        public static bool TryGetProductInfo(Transform transform, out ProductInfo info) =>
            info = productInstances.Find(identifier => identifier.transform == transform).productInfo;

        void Awake()
        {
            for (var i = 0; i < allProducts.Length; i++)
                allProducts[i].Init(i);
        }
    }
}
