using Checkout;
using UnityEngine;
using Interactables.Base;

namespace Products
{
    public class ProductIdentifier : MonoBehaviour, ISecondaryInteractHandler, IStopSecondaryInteractHandler
    {
        [field: SerializeField, Min(1)] public Vector2Int Size { get; private set; } = Vector2Int.one;
        public ProductInfo productInfo;

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            var unit = ItemArea.UnitSize;
            var center = transform.position + new Vector3((Size.x + 1) % 2, 0, (Size.y + 1) % 2) / 2 * unit;
            for (var x = 0; x < Size.x; x++)
            for (var y = 0; y < Size.y; y++)
                Gizmos.DrawWireCube(
                    center + transform.rotation * new Vector3(x - Size.x / 2, 0, y - Size.y / 2) * unit,
                    new Vector3(1, 0, 1) * unit);
        }

        void OnEnable() => ProductManager.RegisterProduct(this);
        void OnDisable() => ProductManager.DeregisterProduct(this);

        void Awake()
        {
            if (!CompareTag("Product"))
                Debug.LogWarning(name + " does not have the Product tag. Set it after playing.", gameObject);
            tag = "Product";
        }

        public void OnSecondaryInteract(Transform sender) =>
            BarcodeDisplay.Main.ShowBarcodeFor(productInfo);

        public void OnStopSecondaryInteract(Transform sender) =>
            BarcodeDisplay.Main.HideBarcode();
    }
}
