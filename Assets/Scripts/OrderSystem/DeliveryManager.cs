using System;
using System.Collections;
using System.Collections.Generic;
using Products;
using UnityEngine;

namespace OrderSystem
{
    public class DeliveryManager : MonoBehaviour
    {
        [SerializeField] float secondsPerItem;
        
        readonly List<Shipment> shipments = new();

        IEnumerator TimeShipments()
        {
            while (shipments.Count > 0)
            {
                yield return new WaitForSeconds(1);
                
                // Collection gets modified outside loop so can't use foreach enumeration
                for (var i = 0; i < shipments.Count; i++)
                    shipments[i].IncreaseProgress();
            }
        }

        public int EstimateDeliveryTime(int numItems) => Mathf.RoundToInt(numItems * secondsPerItem);

        public void CreateShipment(ShipmentItem[] items)
        {
            var total = 0;
            foreach (var item in items)
                total += item.quantity;
            
            var time = EstimateDeliveryTime(total);
            
            shipments.Add(new Shipment(items, time, Deliver));

            if (shipments.Count == 1)
                StartCoroutine(TimeShipments());
        }

        void Deliver(Shipment shipment)
        {
            shipments.Remove(shipment);
            
            Debug.Log("New shipment has been delivered: " + string.Join(", ", shipment.items));
        }
    }

    public readonly struct ShipmentItem
    {
        public readonly ProductInfo product;
        public readonly int quantity;

        public ShipmentItem(ProductInfo product, int quantity)
        {
            this.product = product;
            this.quantity = quantity;
        }

        public override string ToString() => $"{product} x {quantity}";
    }

    internal class Shipment
    {
        public int ShippingProgress { get; private set; }
        
        public readonly ShipmentItem[] items;
        public readonly int shippingTotalTime;

        readonly Action<Shipment> delivered;

        public Shipment(ShipmentItem[] items, int shipTime, Action<Shipment> onDeliver)
        {
            this.items = items;
            delivered = onDeliver;
            shippingTotalTime = shipTime;
        }

        public void IncreaseProgress()
        {
            ShippingProgress++;
            if (ShippingProgress == shippingTotalTime)
                delivered?.Invoke(this);
        }
    }
}
