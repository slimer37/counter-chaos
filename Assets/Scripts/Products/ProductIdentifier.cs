using UnityEngine;
using Interactables.Base;

namespace Products
{
    public class ProductIdentifier : MonoBehaviour, ISecondaryInteractHandler, IStopSecondaryInteractHandler
    {
        public ProductInfo productInfo;

        BarcodeDisplay currentBarcodeDisplay;

        void Awake()
        {
            if (!CompareTag("Product"))
                Debug.LogWarning(name + " does not have the Product tag. Set it after playing.", gameObject);
            tag = "Product";
        }

        public void OnSecondaryInteract(Transform sender) =>
            (currentBarcodeDisplay = sender.GetComponent<BarcodeDisplay>()).ShowBarcodeFor(productInfo);

        public void OnStopSecondaryInteract(Transform sender) =>
            currentBarcodeDisplay?.HideBarcode();
    }
}
