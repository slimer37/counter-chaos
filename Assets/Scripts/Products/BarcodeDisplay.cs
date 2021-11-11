using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Products
{
    public class BarcodeDisplay : MonoBehaviour
    {
        [SerializeField] CanvasGroup uiGroup;
        [SerializeField] RawImage image;
        [SerializeField] TextMeshProUGUI id;
        [SerializeField] TextMeshProUGUI shortNameLabel;
        [SerializeField] float fadeTime;
        
        [Header("Sizing")]
        [SerializeField] float textSizeWithBarcode;
        [SerializeField] float textSizeWithoutBarcode;
        
        public static BarcodeDisplay Main { get; private set; }

        void Awake()
        {
            Main = this;
            uiGroup.alpha = 0;
        }

        public void ShowBarcodeFor(ProductInfo info)
        {
            uiGroup.DOComplete();
            uiGroup.DOFade(1, fadeTime);
            
            image.gameObject.SetActive(info.HasBarcode);
            id.fontSize = info.HasBarcode ? textSizeWithBarcode : textSizeWithoutBarcode;
            if (info.HasBarcode)
                image.texture = info.Barcode;

            shortNameLabel.text = info.CompactName;
            id.text = info.ID.ToString();
        }

        public void HideBarcode() => uiGroup.DOFade(0, fadeTime);
    }
}
