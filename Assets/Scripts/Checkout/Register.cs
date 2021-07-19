using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Products;

namespace Checkout
{
    public class Register : MonoBehaviour
    {
        enum Mode { Transaction, Inquiry, Entry }
        
        [SerializeField] TextMeshPro screenText;
        [SerializeField] TextMeshPro bottomText;
        [SerializeField] Scanner scanner;
        [SerializeField] float headerSize;

        [Header("Transaction Mode")]
        [SerializeField] string transactionHeader;
        [SerializeField, Tooltip("{0-1}: Name, Price")] string itemListFormat;
        [SerializeField, TextArea, Tooltip("{0-1}: # of items, Total")] string totalInfoFormat;
        [SerializeField] int maxItemsShown;
        [SerializeField] string truncationString;

        [Header("Manual Entry")]
        [SerializeField] string entryHeader;
        [SerializeField] float inputSize;
        [SerializeField, TextArea] string inputInstructions;
        [SerializeField] GameObject enableDuringEntry;
        
        [Header("Inquiry Mode")]
        [SerializeField] string productInquiryHeader;
        [SerializeField, TextArea, Tooltip("{0-2}: Name, Price, Description")] string productInquiryFormat;

        readonly List<ProductInfo> transactionItems = new List<ProductInfo>();

        Mode mode = Mode.Transaction;
        string idInput;

        void Awake()
        {
            enableDuringEntry.SetActive(false);
            scanner.OnScan += OnScan;
            FormatModeText();
        }

        void OnScan(ProductIdentifier productIdentifier)
        {
            if (mode == Mode.Entry) return;
            
            var info = productIdentifier.productInfo;
            
            if (mode == Mode.Transaction)
                transactionItems.Add(info);

            FormatModeText();

            if (mode == Mode.Inquiry)
                screenText.text += string.Format(productInquiryFormat,
                    info.DisplayName, info.Price.ToString("c"), info.Description);
        }

        public void InputID(char? c)
        {
            if (mode != Mode.Entry) return;
            
            if (c == null && idInput.Length > 0) idInput = idInput.Remove(idInput.Length - 1);
            else if (idInput.Length < ProductInfo.IDLength) idInput += c;
            
            FormatModeText();
        }
        
        public void ToggleInquireMode() => ActivateMode(Mode.Inquiry);
        public void ToggleEntryMode() => ActivateMode(Mode.Entry);
        
        void ActivateMode(Mode newMode)
        {
            // Return to Transaction mode if newMode is the current mode.
            mode = mode == newMode ? Mode.Transaction : newMode;

            if (mode == Mode.Entry) idInput = "";
            enableDuringEntry.SetActive(mode == Mode.Entry);
            
            FormatModeText();
        }

        public void UndoLastItem() => EditTransaction(() => transactionItems.RemoveAt(transactionItems.Count - 1));
        public void ClearTransaction() => EditTransaction(() => transactionItems.Clear());

        void EditTransaction(Action editAction)
        {
            if (mode != Mode.Transaction || transactionItems.Count == 0) return;
            editAction();
            FormatModeText();
        }

        void AppendTransactionText()
        {
            var displayList = transactionItems;
            
            // Add ellipsis and trim beginning of display list if there are too many items to show.
            var numItems = transactionItems.Count;
            if (numItems > maxItemsShown)
            {
                screenText.text += truncationString + "\n";
                displayList = transactionItems.GetRange(numItems - maxItemsShown, maxItemsShown);
            }
            
            displayList.ForEach(item =>
                screenText.text += string.Format(itemListFormat, item.DisplayName, item.Price.ToString("c")) + "\n");
            
            var total = transactionItems.Sum(item => item.Price);
            bottomText.text = string.Format(totalInfoFormat, transactionItems.Count, total.ToString("c"));
        }

        void FormatModeText()
        {
            var header = mode switch
            {
                Mode.Inquiry => productInquiryHeader,
                Mode.Transaction => transactionHeader,
                Mode.Entry => entryHeader,
                _ => throw new Exception("Invalid mode.")
            };
            
            screenText.text = $"<size={headerSize}>{header}</size>\n";
            bottomText.text = "";

            if (mode == Mode.Transaction)
                AppendTransactionText();
            else if (mode == Mode.Entry) screenText.text += $"\n<size={inputSize}>" + idInput + "</size>\n\n" + inputInstructions;
        }
    }
}
