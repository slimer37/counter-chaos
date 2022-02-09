using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Products.Browser
{
    internal class ProductPageUI : MonoBehaviour
    {
        [SerializeField] GameObject infoPage;
        [SerializeField] Button backButton;
        [SerializeField] Button addToCartButton;
        [SerializeField] TMP_InputField quantityField;
        [SerializeField] ProductBrowserCart cart;
        
        [Header("Text")]
        [SerializeField] TMP_Text productName;
        [SerializeField] TMP_Text infoLine;
        [SerializeField] TMP_Text price;
        [SerializeField] TMP_Text description;
        [SerializeField] GameObject successBox;

        [Header("Preview")]
        [SerializeField] Vector3 startingRotation = Vector3.up * 180;
        [SerializeField] RawImage image;
        [SerializeField] Camera cam;
        [SerializeField] Transform productPosition;
        [SerializeField] Vector2Int size;
        [SerializeField, Range(0, 1)] float padding;
        [SerializeField, Min(0)] float back;

        GameObject preview;
        ProductInfo currentSubject;
        
        void Awake()
        {
            addToCartButton.onClick.AddListener(AddToCart);
            addToCartButton.interactable = false;
            
            quantityField.onValueChanged.AddListener(QuantityChanged);
            
            backButton.onClick.AddListener(Hide);
            infoPage.SetActive(false);
            successBox.SetActive(false);

            cam.enabled = false;
            cam.targetTexture = new RenderTexture(size.x, size.y, 16);
            image.texture = cam.targetTexture;
        }

        void QuantityChanged(string text) => addToCartButton.interactable = !string.IsNullOrEmpty(text);

        void AddToCart()
        {
            var quantity = int.Parse(quantityField.text);
            cart.AddItemToCart(currentSubject, quantity);
            
            successBox.SetActive(true);
        }

        public void View(ProductInfo info)
        {
            currentSubject = info;
            
            infoPage.SetActive(true);

            productName.text = info.DisplayName;
            price.text = info.Price.ToString("C");
            infoLine.text = $"ID: #{info.ID}";
            description.text = info.Description;

            productPosition.rotation = Quaternion.identity;
            
            preview = info.Instantiate();
            preview.GetComponentInChildren<Rigidbody>().useGravity = false;
            preview.transform.SetParent(productPosition);
            productPosition.transform.localRotation = Quaternion.Euler(startingRotation);

            var filter = preview.GetComponentInChildren<MeshFilter>();
            var bounds = filter.sharedMesh.bounds;
            
            var extents = bounds.extents;
            extents.Scale(preview.transform.lossyScale);
            bounds.extents = extents;
            
            preview.transform.localPosition = bounds.center;

            bounds.center += preview.transform.position;
            
            RuntimePreviewGenerator.CalculateCameraPosition(cam, bounds, padding);
            cam.transform.Translate(Vector3.back * back, Space.Self);
            
            cam.enabled = true;
        }

        void Hide()
        {
            currentSubject = null;
            
            infoPage.SetActive(false);
            successBox.SetActive(false);
            Destroy(preview);
            cam.enabled = false;
        }
    }
}
