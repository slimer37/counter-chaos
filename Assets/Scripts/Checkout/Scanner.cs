using UnityEngine;

namespace Checkout
{
    public class Scanner : MonoBehaviour
    {
        [SerializeField] LayerMask raycastMask;
        [SerializeField] Vector3 laserDirection = Vector3.up;
        [SerializeField] float maxDistance = 0.5f;

        Ray scanRay;
        Transform lastScanned;

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + laserDirection * maxDistance);
        }

        void Awake() => scanRay = new Ray(transform.position, laserDirection);

        void Scan(Transform scanned)
        {
            lastScanned = scanned;
            print(scanned);
        }

        void FixedUpdate()
        {
            if (Physics.Raycast(scanRay, out var hit, maxDistance, raycastMask))
            {
                if (lastScanned == hit.transform) return;
                Scan(hit.transform);
            }
            else
                lastScanned = null;
        }
    }
}
