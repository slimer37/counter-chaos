using UnityEngine;
using Interactables.Base;

namespace Products
{
    public class ProductIdentifier : MonoBehaviour, ISecondaryInteractHandler, IStopSecondaryInteractHandler
    {
        public ProductInfo productInfo;

        BarcodeDisplay currentBarcodeDisplay;
        
        public void OnSecondaryInteract(Transform sender) =>
            (currentBarcodeDisplay = sender.GetComponent<BarcodeDisplay>()).ShowBarcodeFor(productInfo);

        public void OnStopSecondaryInteract(Transform sender) =>
            currentBarcodeDisplay?.HideBarcode();
    }
}
