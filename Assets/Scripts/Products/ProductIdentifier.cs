using UnityEngine;
using Interactables.Base;

namespace Products
{
    public class ProductIdentifier : MonoBehaviour, ISecondaryInteractable
    {
        public ProductInfo productInfo;

        void OnEnable() => ProductLibrary.AddInstance(this);
        void OnDisable() => ProductLibrary.RemoveInstance(this);

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
