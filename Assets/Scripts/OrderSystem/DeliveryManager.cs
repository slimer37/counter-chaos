﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace OrderSystem
{
    public class DeliveryManager : MonoBehaviour
    {
        [SerializeField] float secondsPerItem;

        public ReadOnlyCollection<Shipment> Shipments => shipments.AsReadOnly();

        readonly List<Shipment> shipments = new();

        public event Action<Shipment> Shipped;

        IEnumerator TimeShipments()
        {
            while (shipments.Count > 0)
            {
                yield return new WaitForSeconds(1);
                
                // Collection gets modified outside loop so can't use foreach enumeration
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < shipments.Count; i++)
                    shipments[i].IncreaseProgress();
            }
        }

        public int EstimateDeliveryTime(int numItems) => Mathf.RoundToInt(numItems * secondsPerItem);

        public void CreateShipment(ShipmentItem[] items, Texture2D thumbnail)
        {
            var total = 0;
            foreach (var item in items)
                total += item.quantity;
            
            var time = EstimateDeliveryTime(total);
            
            var shipment = new Shipment(items, time, thumbnail);
            shipment.Delivered += Deliver;
            
            shipments.Add(shipment);

            if (shipments.Count == 1)
                StartCoroutine(TimeShipments());
            
            Shipped?.Invoke(shipment);
        }

        void Deliver(Shipment shipment)
        {
            shipments.Remove(shipment);
            
            Debug.Log("New shipment has been delivered: " + string.Join(", ", shipment.items));
        }
    }
}
