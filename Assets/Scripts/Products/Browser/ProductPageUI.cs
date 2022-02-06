using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Products.Browser
{
    internal class ProductPageUI : MonoBehaviour
    {
        [SerializeField] GameObject infoPage;
        [SerializeField] Button backButton;
        [SerializeField] RawImage image;
        
        [Header("Text")]
        [SerializeField] TMP_Text productName;
        [SerializeField] TMP_Text infoLine;
        [SerializeField] TMP_Text price;
        [SerializeField] TMP_Text description;

        void Awake()
        {
            backButton.onClick.AddListener(Hide);
        }

        public void View(ProductInfo info)
        {
            infoPage.SetActive(true);

            productName.text = info.DisplayName;
            price.text = info.Price.ToString("C");
            infoLine.text = $"ID: #{info.ID}";
            description.text = info.Description;
            
            image.texture = Preview.Thumbnail.Grab(info.DisplayName, null);
        }

        void Hide()
        {
            infoPage.SetActive(false);
        }
    }
}
