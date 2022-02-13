using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace OrderSystem.UI
{
    public class OrderTrackerUI : MonoBehaviour
    {
        [SerializeField] DeliveryManager deliveryManager;
        [SerializeField] Transform listParent;

        [Header("Prefab")]
        [SerializeField] TrackedShipmentUI trackedShipmentUIPrefab;
        
        [Header("Details")]
        [SerializeField] TMP_Text detailsText;
        [SerializeField,
         Tooltip("{0} - total, {1} - delivery progress, {2} - delivery total time"),
         TextArea(5, 10)] string detailsFormat;
        [SerializeField] string unselectedText;
        [SerializeField] TMP_Text contentsText;

        string progressPath;

        readonly List<Shipment> trackedShipments = new();

        Shipment focusedShipment;

        void Awake()
        {
            trackedShipmentUIPrefab.gameObject.SetActive(false);
            
            contentsText.text = "";
            detailsText.text = unselectedText;
        }

        void OnEnable()
        {
            foreach (var shipment in deliveryManager.Shipments)
            {
                if (trackedShipments.Contains(shipment)) continue;
                
                TrackNewShipment(shipment);
            }
        }

        void TrackNewShipment(Shipment shipment)
        {
            var clone = Instantiate(trackedShipmentUIPrefab, listParent);
            clone.gameObject.SetActive(true);
            clone.SetShipment(shipment);
            clone.Click += ShowDetails;
        }

        void ShowDetails(Shipment shipment)
        {
            focusedShipment = shipment;
            
            UpdateDetailsText();

            contentsText.text = "";

            foreach (var item in shipment.items)
                contentsText.text += $"- {item}\n";
        }

        void UpdateDetailsText()
        {
            var shipment = focusedShipment;
            
            detailsText.text =
                string.Format(detailsFormat,
                    shipment.totalCost.ToString("C"),
                    shipment.ShippingProgress,
                    shipment.shippingTotalTime);
        }

        void Update()
        {
            if (focusedShipment == null) return;
            
            UpdateDetailsText();
        }
    }
}
