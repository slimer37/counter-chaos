using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;
using Products;

namespace Checkout
{
    public class Register : MonoBehaviour
    {
        enum Mode { Transaction, Inquiry }
        
        [SerializeField] TextMeshPro screenText;
        [SerializeField] TextMeshPro bottomText;
        [SerializeField] Scanner scanner;
        [SerializeField] float headerSize;

        [Header("Transaction Mode")]
        [SerializeField] float itemListSize;
        [SerializeField] float totalAmountSize;
        [SerializeField] int maxItemsShown;
        [SerializeField] float ellipsisSize;
        [SerializeField, TextArea] string transactionHeader = "TRANSACTION MODE";
        
        [Header("Inquiry Mode")]
        [SerializeField, TextArea] string productInquiryHeader = "INQUIRE MODE";
        [SerializeField, TextArea] string productInquiryFormat = "{0}\\n<size=0.4>{1}\\n{2}";

        List<ProductInfo> transactionItems = new List<ProductInfo>();

        Mode mode = Mode.Transaction;

        bool IsInTransaction => mode == Mode.Transaction && transactionItems.Count > 0;

        string GetTotalFormat(float price) => $"<size={totalAmountSize}><align=right>-----\nTOTAL: {price:c}</align></size>";

        void OnValidate()
        {
            // If the header doesn't contain {0} through {2}, log a warning.
            if (!new[] {"{0}", "{1}", "{2}"}.All(productInquiryFormat.Contains))
                Debug.LogWarning(nameof(productInquiryFormat) + " does not contain format items {0} through {2}.",
                    gameObject);

            screenText.text = $"<size={headerSize}>HEADER</size>\n" +
                        $"<size={ellipsisSize}>...</size>\n" +
                        $"<size={itemListSize}>Item - Price</size>\n" +
                        string.Format(Regex.Unescape(productInquiryFormat), "Inquiry Sample", "Price", "Description");

            bottomText.text = GetTotalFormat(0);
        }

        void Awake()
        {
            scanner.onScan += OnScan;
            FormatModeText();
        }

        void OnScan(ProductIdentifier productIdentifier)
        {
            var info = productIdentifier.productInfo;
            FormatModeText();

            if (mode == Mode.Inquiry)
            {
                screenText.text += string.Format(Regex.Unescape(productInquiryFormat),
                    info.DisplayName,info.Price.ToString("c"), info.Description);
            }
            else if (mode == Mode.Transaction)
            {
                transactionItems.Add(info);
                ShowTransactionText();
            }
        }

        public void ToggleMode()
        {
            var numModes = Enum.GetNames(typeof(Mode)).Length;
            mode = (Mode)(((int)mode + 1) % numModes);
            
            FormatModeText();
            
            if (mode == Mode.Transaction)
                ShowTransactionText();
        }

        public void UndoLastItem() => EditTransaction(() => transactionItems.RemoveAt(transactionItems.Count - 1));
        public void ClearTransaction() => EditTransaction(() => transactionItems.Clear());

        void EditTransaction(Action editAction)
        {
            if (!IsInTransaction) return;
            editAction();
            FormatModeText();
            ShowTransactionText();
        }

        void ShowTransactionText()
        {
            var numItems = transactionItems.Count;
            var displayList = transactionItems;
            screenText.text += $"<size={itemListSize}>";
            
            // Add ellipsis and trim beginning of display list if there are too many items to show.
            if (numItems > maxItemsShown)
            {
                screenText.text += $"<size={ellipsisSize}>...</size>\n";
                displayList = transactionItems.GetRange(numItems - maxItemsShown, maxItemsShown);
            }
            
            displayList.ForEach(item =>
                screenText.text += $"{item.DisplayName} - {item.Price:c}\n");
            
            var total = transactionItems.Sum(item => item.Price);
            bottomText.text = GetTotalFormat(total);
        }

        void FormatModeText()
        {
            var header = mode switch
            {
                Mode.Inquiry => productInquiryHeader,
                Mode.Transaction => transactionHeader,
                _ => throw new Exception("Invalid mode.")
            };
            
            screenText.text = $"<size={headerSize}>{header}</size>\n";

            bottomText.text = mode == Mode.Transaction ? GetTotalFormat(0) : "";
        }
    }
}
