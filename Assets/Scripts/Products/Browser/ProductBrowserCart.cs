using System.Collections.Generic;
using Preview;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Products.Browser
{
    public class ProductBrowserCart : MonoBehaviour
    {
        [SerializeField] Transform listParent;
        [SerializeField] Button placeOrderButton;
        
        [Header("List Item Prefab")]
        [SerializeField] GameObject itemPrefab;
        [SerializeField] RawImage image;
        [SerializeField] TMP_Text leftText;
        [SerializeField] string rightTextPath;
        [SerializeField] string subtractButtonPath, addButtonPath, removeButtonPath;
        
        List<CartItem> contents = new();

        void Awake()
        {
            placeOrderButton.onClick.AddListener(PlaceOrder);
            itemPrefab.SetActive(false);
        }

        public void AddItemToCart(ProductInfo product, int quantity)
        {
            image.texture = Thumbnail.Grab(product.DisplayName, null);
            leftText.GetComponent<TMP_Text>().text = $"{product.DisplayName} (#{product.ID.ToString()}) ({product.Price:C})";
            
            var clone = Instantiate(itemPrefab, listParent).transform;
            
            var rightText = clone.transform.Find(rightTextPath).GetComponent<TMP_Text>();
            var add = clone.transform.Find(addButtonPath).GetComponent<Button>();
            var subtract = clone.transform.Find(subtractButtonPath).GetComponent<Button>();
            var remove = clone.transform.Find(removeButtonPath).GetComponent<Button>();
            
            var listItem = new CartItem(product, quantity) {uiItem = clone.gameObject, dataText = rightText};
            listItem.UpdateText();

            add.onClick.AddListener(() => listItem.Quantity++);
            subtract.onClick.AddListener(() => listItem.Quantity--);
            remove.onClick.AddListener(() => {
                contents.Remove(listItem);
                Destroy(listItem.uiItem);
            });

            contents.Add(listItem);
            clone.gameObject.SetActive(true);
        }

        public void PlaceOrder()
        {
            var total = 0f;
            
            foreach (var item in contents)
            {
                total += item.Total;
                Destroy(item.uiItem);
            }
            
            Debug.Log(total);
            contents.Clear();
        }
    }
    
    internal class CartItem
    {
        public ProductInfo Product { get; }
        public float Total { get; private set; }

        public GameObject uiItem;
        public TMP_Text dataText;
        
        public const int MaximumQuantity = 100;
        
        public int Quantity
        {
            get => quantity;
            set
            {
                SetQuantity(value);
                UpdateText();
            }
        }

        int quantity;
        
        public void UpdateText()
        {
            if (!dataText)
            {
                Debug.LogWarning("No data text is assigned. Item data cannot be shown.");
                return;
            }
            
            dataText.text = $"{Total:C} | [QTY: {Quantity}]";
        }

        void SetQuantity(int value)
        {
            quantity = Mathf.Clamp(value, 1, MaximumQuantity);
            Total = Product.Price * quantity;
        }

        public CartItem(ProductInfo product, int quantity)
        {
            Product = product;
            SetQuantity(quantity);
        }
    }
}
