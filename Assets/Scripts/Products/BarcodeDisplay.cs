using Products;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BarcodeDisplay : MonoBehaviour
{
    [SerializeField] GameObject uiParent;
    [SerializeField] RawImage image;
    [SerializeField] TextMeshProUGUI id;

    void Awake() => uiParent.SetActive(false);

    public void ShowBarcodeFor(ProductInfo info)
    {
        uiParent.SetActive(true);
        image.texture = info.Barcode;
        id.text = info.ID.ToString();
    }

    public void HideBarcode() => uiParent.SetActive(false);
}
