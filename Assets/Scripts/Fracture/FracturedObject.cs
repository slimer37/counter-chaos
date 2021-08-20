using System.Collections;
using UnityEngine;

public class FracturedObject : MonoBehaviour
{
    [SerializeField] Rigidbody[] rigidbodies;
    float delay;

    void OnValidate()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.layer != LayerMask.NameToLayer("Fracture Piece"))
            {
                Debug.LogWarning($"All children of {name} must be on the Fracture Piece layer.");
                return;
            }
        }
    }

    void Reset() => rigidbodies = GetComponentsInChildren<Rigidbody>();

    internal void Explode(Vector3 collidePoint, float force, float destroyDelay)
    {
        foreach (var rb in rigidbodies)
            rb.AddExplosionForce(force, collidePoint, 0);
        
        delay = destroyDelay;
        StartCoroutine(DelayedOut());
    }

    IEnumerator DelayedOut()
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
