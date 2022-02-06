using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UI.Tooltip;

namespace Products.Browser
{
    internal class ProductBrowserUI : MonoBehaviour
    {
        [SerializeField] GameObject productButtonPrefab;
        [SerializeField] Transform itemParent;
        [SerializeField] ProductPageUI productPage;
        [SerializeField, TextArea] string itemStringFormat;
        [SerializeField] Vector2Int imageSize;
        
        [Header("Search")]
        [SerializeField] TMP_InputField searchBar;
        [SerializeField] GameObject loadingGraphic;

        bool searchInProgress;
        bool[] queueInactive;

        void Awake()
        {
            searchBar.onValueChanged.AddListener(s => SearchLibrary());
        }

        void Start()
        {
            productButtonPrefab.SetActive(false);
            Populate();
        }

        void SearchLibrary()
        {
            if (searchInProgress) StopAllCoroutines();
            
            StartCoroutine(DoSearch(searchBar.text));
        }

        IEnumerator DoSearch(string query)
        {
            SetSearching(true);

            query = query.ToLowerInvariant();
            var emptySearch = string.IsNullOrWhiteSpace(query);

            var i = 0;

            foreach (Transform item in itemParent)
            {
                if (item.gameObject == productButtonPrefab) continue;

                queueInactive[i++] = emptySearch || item.name.Contains(query);
                
                yield return null;
            }

            i = 0;
            
            foreach (Transform item in itemParent)
            {
                if (item.gameObject == productButtonPrefab) continue;

                item.gameObject.SetActive(queueInactive[i++]);
            }
            
            SetSearching(false);

            void SetSearching(bool value)
            {
                searchInProgress = value;
                loadingGraphic.SetActive(false);
            }
        }

        void Populate()
        {
            queueInactive = new bool[ProductLibrary.AllProducts.Count];
            
            foreach (var product in ProductLibrary.AllProducts)
            {
                var clone = Instantiate(productButtonPrefab, itemParent);
                
                clone.AddComponent<TooltipTrigger>().TitleText = product.DisplayName;
                
                try
                {
                    // Used for searches
                    clone.name = product.DisplayName.ToLowerInvariant();
                    
                    var img = clone.GetComponentInChildren<RawImage>();
                    var productClone = product.Instantiate();
                    img.texture = Preview.Thumbnail.Grab(product.DisplayName, productClone.transform, imageSize);
                    Destroy(productClone);
                    clone.SetActive(true);
                }
                catch (System.Exception e)
                {
                    throw new System.Exception($"Encountered exception while loading \"{product.DisplayName}\".", e);
                }

                clone.GetComponentInChildren<Button>().onClick.AddListener(() => productPage.View(product));
                clone.GetComponentInChildren<TMP_Text>().text = string.Format(itemStringFormat,
                    product.Price.ToString("C"), product.DisplayName);
            }
        }
    }
}
