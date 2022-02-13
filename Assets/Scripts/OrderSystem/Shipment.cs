using System;
using Products;
using UnityEngine;

namespace OrderSystem
{
    public class Shipment
    {
        public int ShippingProgress { get; private set; }
        public bool WasDelivered { get; private set; }
        
        public readonly ShipmentItem[] items;
        public readonly int shippingTotalTime;
        public readonly float totalCost;
        public readonly Texture2D thumbnail;

        public event Action<Shipment> Delivered;

        internal Shipment(ShipmentItem[] items, int shipTime, Texture2D thumbnail)
        {
            this.items = items;
            this.thumbnail = thumbnail;
            shippingTotalTime = shipTime;

            foreach (var item in items)
                totalCost += item.product.Price * item.quantity;
        }

        internal void IncreaseProgress()
        {
            if (WasDelivered) return;
            
            ShippingProgress++;
            
            if (ShippingProgress == shippingTotalTime)
            {
                WasDelivered = true;
                Delivered?.Invoke(this);
            }
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
}
