using System.Collections.Generic;
using UnityEngine;

namespace OrderSystem.UI
{
    public class OrderTrackerUI : MonoBehaviour
    {
        [SerializeField] DeliveryManager deliveryManager;
        [SerializeField] Transform listParent;

        [Header("Prefab")]
        [SerializeField] TrackedShipmentUI trackedShipmentUIPrefab;

        string progressPath;

        readonly List<Shipment> trackedShipments = new();

        void Awake()
        {
            trackedShipmentUIPrefab.gameObject.SetActive(false);
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
        }
    }
}
