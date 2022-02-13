using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OrderSystem.UI
{
    internal class TrackedShipmentUI : MonoBehaviour
    {
        [SerializeField] Button button;
        [SerializeField] Slider progress;
        [SerializeField] TMP_Text deliveredText;
        [SerializeField] TMP_Text detailsText;
        [SerializeField] RawImage image;

        public event Action<Shipment> Click;

        Shipment shipment;

        void Awake()
        {
            button.onClick.AddListener(() => Click?.Invoke(shipment));
        }

        public void SetShipment(Shipment value)
        {
            shipment = value;
            
            deliveredText.gameObject.SetActive(value.WasDelivered);
            value.Delivered += _ => deliveredText.gameObject.SetActive(true);

            detailsText.text = shipment.totalCost.ToString("C");

            image.texture = shipment.thumbnail;
            
            progress.minValue = 0;
            progress.maxValue = shipment.shippingTotalTime;
        }

        void Update()
        {
            if (shipment == null) return;
            progress.value = shipment.ShippingProgress;
        }
    }
}
