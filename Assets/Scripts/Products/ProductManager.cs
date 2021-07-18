using UnityEngine;

namespace Products
{
    public class ProductManager : MonoBehaviour
    {
        [SerializeField] ProductInfo[] allProducts;
        
        void Awake()
        {
            for (var i = 0; i < allProducts.Length; i++)
                allProducts[i].Init(i);
        }
    }
}
