using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;
using Products;

namespace Checkout
{
    public class Register : MonoBehaviour
    {
        [SerializeField, TextArea] string productInquiryHeader = "<size=25%>INQUIRE MODE</size>\\n";
        [SerializeField, TextArea] string productInquiryFormat = "{0}\\n<size=50%>{1}\\n{2}";
        [SerializeField] TextMeshPro text;
        [SerializeField] Scanner scanner;

        bool inInquireMode;

        void OnValidate()
        {
            // If the header doesn't contain {0} through {2}, log a warning.
            if (!new[] {"{0}", "{1}", "{2}"}.All(productInquiryFormat.Contains))
                Debug.LogWarning(nameof(productInquiryFormat) + " does not contain format items {0} through {2}.",
                    gameObject);
        }

        void Awake()
        {
            scanner.onScan += OnScan;
            text.text = "";
            productInquiryHeader = Regex.Unescape(productInquiryHeader);
            productInquiryFormat = Regex.Unescape(productInquiryFormat);
        }

        void OnScan(ProductIdentifier productIdentifier)
        {
            var info = productIdentifier.productInfo;
            
            if (inInquireMode)
            {
                text.text = productInquiryHeader;
                text.text += string.Format(productInquiryFormat, info.DisplayName, info.Price.ToString("c"), info.Description);
            }
        }

        public void ToggleInquiry()
        {
            inInquireMode = !inInquireMode;

            text.text = inInquireMode ? productInquiryHeader : "";
        }
    }
}
