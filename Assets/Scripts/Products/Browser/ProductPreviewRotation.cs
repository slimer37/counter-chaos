using UnityEngine;
using UnityEngine.EventSystems;

namespace Products.Browser
{
    public class ProductPreviewRotation : MonoBehaviour, IDragHandler, IPointerDownHandler, IEndDragHandler
    {
        [SerializeField] float sensitivity = 1;
        [SerializeField] float idleSpeed = 1;
        [SerializeField] float speedUpTime = 1;
        [SerializeField] float idleTime = 1;
        [SerializeField] Transform rotator;

        bool freeze;
        bool idle;

        float timeIdling;
        float t;

        void OnEnable() => idle = true;

        void Update()
        {
            if (freeze) return;
            
            if (!idle)
            {
                timeIdling += Time.deltaTime;
                
                if (timeIdling > idleTime)
                {
                    idle = true;
                    t = 0;
                }
                
                return;
            }
            
            rotator.Rotate(-Mathf.Lerp(0, idleSpeed, t) * Time.deltaTime * Vector3.up);
            t += Time.deltaTime / speedUpTime;
        }

        public void OnPointerDown(PointerEventData eventData) => freeze = true;
        public void OnDrag(PointerEventData eventData) =>
            rotator.Rotate(-eventData.delta.x * sensitivity * Time.deltaTime * Vector3.up);
        public void OnEndDrag(PointerEventData eventData)
        {
            freeze = false;
            idle = false;
            timeIdling = 0;
        }
    }
}
