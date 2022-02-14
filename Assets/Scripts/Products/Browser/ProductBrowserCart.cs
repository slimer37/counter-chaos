using System.Collections.Generic;
using OrderSystem;
using Preview;
using TMPro;
using UI.Extensions;
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
        [SerializeField] TMP_Text rightText;
        [SerializeField] Button subtractButton, addButton, removeButton;
        
        readonly List<CartItem> contents = new();

        string rightTextPath, subtractPath, addPath, removePath;

        bool initialized;

        void Awake()
        {
            if (initialized) return;
            Initialize();
        }

        void Initialize()
        {
            if (initialized) Debug.LogWarning("Initialize was called a second time.");
            
            initialized = true;
            
            CachePath(rightText, out rightTextPath);
            CachePath(subtractButton, out subtractPath);
            CachePath(addButton, out addPath);
            CachePath(removeButton, out removePath);
            
            placeOrderButton.onClick.AddListener(PlaceOrder);
            itemPrefab.SetActive(false);

            UpdateDetails();

            void CachePath(Object c, out string cache) =>
                cache = itemPrefab.transform.FindPathRecursive(c.name);
        }

        public void AddItemToCart(ProductInfo product, int quantity)
        {
            if (!initialized) Initialize();
            
            image.texture = Thumbnail.Grab(product.DisplayName, null);
            leftText.GetComponent<TMP_Text>().text = $"{product.DisplayName} (#{product.ID.ToString()}) ({product.Price:C})";
            
            var clone = Instantiate(itemPrefab, listParent).transform;
            
            var listItem = new CartItem(product, quantity) { uiItem = clone.gameObject };
            
            clone.FindComponent(rightTextPath, out listItem.dataText);
            listItem.UpdateText();
            
            clone.FindComponent(addPath, out Button add);
            clone.FindComponent(subtractPath, out Button subtract);
            clone.FindComponent(removePath, out Button remove);

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

        void PlaceOrder()
        {
            var shipmentItems = new ShipmentItem[contents.Count];

            for (var i = 0; i < contents.Count; i++)
            {
                var item = contents[i];
                shipmentItems[i] = new ShipmentItem(item.product, item.Quantity);
                Destroy(item.uiItem);
            }

            deliveryManager.CreateShipment(shipmentItems, Thumbnail.Grab(contents[0].product.DisplayName, null));
            
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
