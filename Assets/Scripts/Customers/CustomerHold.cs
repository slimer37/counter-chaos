using DG.Tweening;
using Interactables.Holding;
using UnityEngine;

namespace Customers
{
    public class CustomerHold : MonoBehaviour
    {
        [SerializeField] Vector3 holdingPosition;
        [SerializeField] float holdAnimDuration;
        
        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0, 1, 0, 0.5f);
            Gizmos.DrawCube(transform.TransformPoint(holdingPosition), Vector3.one * 0.25f);
        }

        internal YieldInstruction Pickup(Pickuppable pickuppable)
        {
            pickuppable.OnInteract(transform);
            return pickuppable.transform.DOLocalMove(holdingPosition, holdAnimDuration).WaitForCompletion();
        }
    }
}
