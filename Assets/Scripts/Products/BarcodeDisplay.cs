using DG.Tweening;
using Products;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BarcodeDisplay : MonoBehaviour
{
    [SerializeField] CanvasGroup uiGroup;
    [SerializeField] RawImage image;
    [SerializeField] TextMeshProUGUI id;
    [SerializeField] float fadeTime;

    void Awake() => uiGroup.alpha = 0;

    public void ShowBarcodeFor(ProductInfo info)
    {
        uiGroup.DOComplete();
        uiGroup.DOFade(1, fadeTime);
        image.texture = info.Barcode;
        id.text = info.ID.ToString();
    }

    public void HideBarcode() => uiGroup.DOFade(0, fadeTime);
}