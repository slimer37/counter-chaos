using System.Collections.Generic;
using UnityEngine;

namespace Products
{
    public class ProductManager : MonoBehaviour
    {
        [SerializeField] ProductInfo[] allProducts;

        static List<ProductIdentifier> productInstances;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init() => productInstances = new List<ProductIdentifier>();

        public static void RegisterProduct(ProductIdentifier productIdentifier) => productInstances.Add(productIdentifier);
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
