using System;
using Products;

namespace OrderSystem
{
    public class Shipment
    {
        public int ShippingProgress { get; private set; }
        
        public readonly ShipmentItem[] items;
        public readonly int shippingTotalTime;

        readonly Action<Shipment> delivered;

        internal Shipment(ShipmentItem[] items, int shipTime, Action<Shipment> onDeliver)
        {
            this.items = items;
            delivered = onDeliver;
            shippingTotalTime = shipTime;
        }

        internal void IncreaseProgress()
        {
            ShippingProgress++;
            if (ShippingProgress == shippingTotalTime)
                delivered?.Invoke(this);
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
