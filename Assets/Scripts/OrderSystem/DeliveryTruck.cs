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
        
        [SerializeField] Vector3 trailerCenter;
        [SerializeField] Vector3 trailerSize;
        [SerializeField] LayerMask mask;

        Sequence deliverSequence;
        
        Shipment currentShipment;

        bool isStopped;

        void OnDrawGizmosSelected()
        {
            if (itemSpawn)
            {
                Gizmos.color = Color.yellow;
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireCube(trailerCenter, trailerSize);
            }
        }

        void Awake()
        {
            deliverSequence = DOTween.Sequence();
            deliverSequence.AppendCallback(() => transform.SetPositionAndRotation(start.position, start.rotation));
            deliverSequence.Append(transform.DOMove(stop.position, speed).SetSpeedBased().SetEase(Ease.OutSine));
            deliverSequence.AppendCallback(FillTrailer);
            deliverSequence.AppendInterval(0.5f); // Let physics register spawned items
            deliverSequence.AppendCallback(() => isStopped = true);
            
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

        void FillTrailer()
        {
            foreach (var item in currentShipment.items)
            {
                for (var i = 0; i < item.quantity; i++)
                {
                    var clone = item.product.Instantiate();
                    clone.transform.position = itemSpawn.position;
                }
            }
        }

        void FixedUpdate()
        {
            if (!isStopped) return;

            if (CheckForEmptyTrailer())
                Leave();
        }

        void Leave()
        {
            isStopped = false;
            transform.DOMove(end.position, speed).SetSpeedBased().SetEase(Ease.InSine);
        }

        bool CheckForEmptyTrailer() =>
            !Physics.CheckBox(transform.TransformPoint(trailerCenter), trailerSize / 2,
                transform.rotation, mask);
    }
}
