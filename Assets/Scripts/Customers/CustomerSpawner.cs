using System.Collections;
using UnityEngine;

namespace Customers
{
    public class CustomerSpawner : MonoBehaviour
    {
        [SerializeField] GameObject prefab;
        [SerializeField] float minInterval;
        [SerializeField] float maxInterval;

        IEnumerator Start()
        {
            while (true)
            {
                Instantiate(prefab, transform.position, Quaternion.identity);
                yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));
            }
        }
    }
}
