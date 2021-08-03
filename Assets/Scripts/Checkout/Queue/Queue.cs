using System;
using System.Collections.Generic;
using UnityEngine;

namespace Checkout
{
    public class Queue : MonoBehaviour
    {
        [SerializeField, Min(1)] int limit = 1;
        [SerializeField] Transform itemDropTransform;
        [SerializeField] Transform itemDropStandPosTransform;

        public Vector3 ItemDropZone => itemDropTransform.position;
        public Vector3 ItemDropStandPos => itemDropStandPosTransform.position;
        
        public int NumCustomersInLine { get; private set; }

        public event Action OnCustomerServed;

        static List<Queue> allQueues;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void InitQueueList() => allQueues = new List<Queue>();

        void Awake() => allQueues.Add(this);

        public static Queue FindClosestQueue(Vector3 closeTo)
        {
            if (allQueues.Count == 0) throw new Exception("No queues found.");
            
            var closestDist = SqrDistance(allQueues[0]);
            var closestQueue = allQueues[0];
            
            foreach (var queue in allQueues)
                if (SqrDistance(queue) < closestDist)
                    closestQueue = queue;

            return closestQueue;

            float SqrDistance(Queue queue) => (queue.transform.position - closeTo).sqrMagnitude;
        }

        public bool TryLineUp()
        {
            if (NumCustomersInLine >= limit) return false;
            NumCustomersInLine++;
            return true;
        }

        public void ServeCustomer()
        {
            if (NumCustomersInLine == 0) throw new InvalidOperationException("ServeCustomer called with no customers in queue.");
            OnCustomerServed?.Invoke();
            NumCustomersInLine--;
        }
    }
}
