using UnityEngine;
using UnityEngine.EventSystems;

namespace Products.Browser
{
    public class ProductPreviewRotation : MonoBehaviour, IDragHandler
    {
        [SerializeField] float sensitivity = 1;
        [SerializeField] Transform rotator;
        
        public void OnDrag(PointerEventData eventData)
        {
            rotator.Rotate(Vector3.up * eventData.delta.x * sensitivity);
        }
    }
}
