using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Fracture
{
    public class Fracture : MonoBehaviour
    {
        [SerializeField] string id;
        [SerializeField] float fractureVelocity;
        [SerializeField] GameObject fracturedPrefab;
        [SerializeField] float destroyFracturedDelay;
        [SerializeField] float explosionForce;

        static readonly Dictionary<string, ObjectPool<FracturedObject>> Pools = new();

        void OnValidate()
        {
            if (!fracturedPrefab.TryGetComponent<FracturedObject>(out _))
                Debug.LogError("Fractured prefab has no FracturedObject component.");
        }

        void Awake()
        {
            if (!Pools.ContainsKey(id))
                Pools[id] = new ObjectPool<FracturedObject>(
                    () => Instantiate(fracturedPrefab).GetComponent<FracturedObject>(),
                    instance => instance.gameObject.SetActive(true));
        }

        void OnCollisionEnter(Collision other)
        {
            if (other.relativeVelocity.sqrMagnitude > fractureVelocity * fractureVelocity)
                SpawnFractured();
        }

        void SpawnFractured()
        {
            var clone = Pools[id].Get();
            clone.transform.position = transform.position;
            clone.transform.rotation = transform.rotation;
            clone.Explode(explosionForce, destroyFracturedDelay, Pools[id].Release);
            Destroy(gameObject);
        }
    }
}
