using DG.Tweening;
using UnityEngine;

namespace OrderSystem
{
    public class DeliveryTruck : MonoBehaviour
    {
        [SerializeField] Transform start;
        [SerializeField] Transform stop;
        [SerializeField] Transform end;
        [SerializeField] Transform itemSpawn;
        [SerializeField] float speed;

        Sequence deliverSequence;
        Shipment currentShipment;

        void Awake()
        {
            deliverSequence = DOTween.Sequence();
            deliverSequence.AppendCallback(() => transform.SetPositionAndRotation(start.position, start.rotation));
            deliverSequence.Append(transform.DOMove(stop.position, speed).SetSpeedBased().SetEase(Ease.OutSine));
            deliverSequence.AppendCallback(() => FillTruckTrailer(currentShipment));
            deliverSequence.Append(transform.DOMove(end.position, speed).SetSpeedBased().SetEase(Ease.InSine));
            deliverSequence.Pause().SetAutoKill(false);
        }

        void Start()
        {
            DeliveryManager.Instance.Delivered += DeliverShipment;
        }

        void DeliverShipment(Shipment shipment)
        {
            currentShipment = shipment;
            deliverSequence.Restart();
        }

        void FillTruckTrailer(Shipment shipment)
        {
            foreach (var item in shipment.items)
            {
                var clone = item.product.Instantiate();
                clone.transform.position = itemSpawn.position;
            }
        }
    }
}
