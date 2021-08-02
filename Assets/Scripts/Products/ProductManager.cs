using UnityEngine;

namespace Products
{
    public class ProductManager : MonoBehaviour
    {
        [SerializeField] ProductInfo[] allProducts;
        
        void Awake()
        {
#if UNITY_EDITOR
            foreach (var identifier in FindObjectsOfType<ProductIdentifier>())
                if (!System.Linq.Enumerable.Contains(allProducts, identifier.productInfo))
                    Debug.LogWarning($"{identifier.productInfo.name} ({identifier.productInfo.DisplayName})" +
                                     $" is not in the product manager.");
#endif
            for (var i = 0; i < allProducts.Length; i++)
                allProducts[i].Init(i);
        }
    }
}
