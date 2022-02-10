using System.Collections.Generic;
using OrderSystem;
using Preview;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Products.Browser
{
    public class ProductBrowserCart : MonoBehaviour
    {
        [SerializeField] DeliveryManager deliveryManager;
        [SerializeField] Transform listParent;
        [SerializeField] Button placeOrderButton;
        [SerializeField] TMP_Text detailsText;
        [SerializeField, TextArea(5, 10), Tooltip("{0} - # items, {1} - delivery time, {2} - total")] string detailsFormat;
        [SerializeField, TextArea] string emptyCartMessage;
        
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

            UpdateDetails();
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

            add.onClick.AddListener(() => ChangeQuantity(listItem, true));
            subtract.onClick.AddListener(() => ChangeQuantity(listItem, false));
            remove.onClick.AddListener(() => RemoveItem(listItem));

            contents.Add(listItem);
            clone.gameObject.SetActive(true);
            
            UpdateDetails();
        }

        void ChangeQuantity(CartItem item, bool increase)
        {
            item.Quantity += increase ? 1 : -1;
            UpdateDetails();
        }

        void RemoveItem(CartItem item)
        {
            contents.Remove(item);
            Destroy(item.uiItem);
            UpdateDetails();
        }

        public void PlaceOrder()
        {
            var shipmentItems = new ShipmentItem[contents.Count];

            for (var i = 0; i < contents.Count; i++)
            {
                var item = contents[i];
                shipmentItems[i] = new ShipmentItem(item.product, item.Quantity);
                Destroy(item.uiItem);
            }

            deliveryManager.CreateShipment(shipmentItems);
            
            contents.Clear();
            
            UpdateDetails();
        }

        void UpdateDetails()
        {
            placeOrderButton.interactable = contents.Count > 0;
            
            if (contents.Count == 0)
            {
                detailsText.text = emptyCartMessage;
                return;
            }
            
            var totalCost = 0f;
            var numItems = 0;
            
            foreach (var item in contents)
            {
                totalCost += item.Total;
                numItems += item.Quantity;
            }
            
            detailsText.text = string.Format(detailsFormat,
                numItems,
                deliveryManager.EstimateDeliveryTime(numItems),
                totalCost.ToString("C"));
        }
    }
    
    internal class CartItem
    {
        public readonly ProductInfo product;
        
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
            Total = product.Price * quantity;
        }

        public CartItem(ProductInfo product, int quantity)
        {
            this.product = product;
            SetQuantity(quantity);
        }
    }
}
