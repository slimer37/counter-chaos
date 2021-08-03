using System;
using System.Collections;
using UnityEngine;
using Products;

namespace Checkout
{
    public class Scanner : MonoBehaviour
    {
        [SerializeField] LayerMask raycastMask;
        [SerializeField] Vector3 laserDirection = Vector3.up;
        [SerializeField] float maxDistance = 0.5f;
        
        [Header("Flash")]
        [SerializeField] Light scanLight;
        [SerializeField] float scanIntensityAdd;
        [SerializeField, Min(0.01f)] float blinkTime = 0.1f;

        public event Action<ProductIdentifier> OnScan;

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
            if (scanned.TryGetComponent(out ProductIdentifier productIdentifier))
                OnScan?.Invoke(productIdentifier);
            
            StopAllCoroutines();
            StartCoroutine(BlinkLight());
        }

        IEnumerator BlinkLight()
        {
            scanLight.intensity += scanIntensityAdd;
            yield return new WaitForSeconds(blinkTime);
            scanLight.intensity -= scanIntensityAdd;
        }

        void FixedUpdate()
        {
            if (Physics.Raycast(scanRay, out var hit, maxDistance, raycastMask))
            {
                if (!hit.transform.CompareTag("Product") || lastScanned == hit.transform) return;
                Scan(hit.transform);
            }
            else
                lastScanned = null;
        }
    }
}
