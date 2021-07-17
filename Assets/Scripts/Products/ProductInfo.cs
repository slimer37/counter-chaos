using UnityEngine;

namespace Products
{
    [CreateAssetMenu(menuName = "Game/Product", fileName = "New Product")]
    public class ProductInfo : ScriptableObject
    {
        [field: SerializeField] public string DisplayName { get; private set; }
        [field: SerializeField] public float Price { get; private set; }
        [field: SerializeField, TextArea] public string Description { get; private set; }
    
        public int ID { get; private set; }
        public Texture2D Barcode { get; private set; }

        public void Init(int id)
        {
            ID = id;
            Barcode = Products.Barcode.Generate(id);
        }
    }
}