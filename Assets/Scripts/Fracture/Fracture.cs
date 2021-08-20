using UnityEngine;

public class Fracture : MonoBehaviour
{
    [SerializeField] float fractureVelocity;
    [SerializeField] GameObject fracturedPrefab;
    [SerializeField] float destroyFracturedDelay;
    [SerializeField] float explosionForce;

    void OnValidate()
    {
        if (!fracturedPrefab.TryGetComponent<FracturedObject>(out _))
            Debug.LogError("Fractured prefab has no FracturedObject component.");
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.relativeVelocity.sqrMagnitude > fractureVelocity * fractureVelocity)
            SpawnFractured();
    }

    void SpawnFractured()
    {
        var clone = Instantiate(fracturedPrefab, transform.position, transform.rotation).GetComponent<FracturedObject>();
        clone.Explode(explosionForce, destroyFracturedDelay);
        Destroy(gameObject);
    }
}
