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
        [SerializeField] float spotSpacing;

        public Vector3 ItemDropZone => itemDropTransform.position;
        public Vector3 ItemDropStandPos => itemDropStandPosTransform.position;
        
        public int NumCustomersInLine { get; private set; }

        public event Action OnCustomerServed;

        static readonly List<Queue> AllQueues = new List<Queue>();

        void OnEnable() => AllQueues.Add(this);
        void OnDisable() => AllQueues.Remove(this);

        public static Queue FindClosestQueue(Vector3 closeTo)
        {
            if (AllQueues.Count == 0) throw new Exception("No queues found.");
            
            var closestDist = SqrDistance(AllQueues[0]);
            var closestQueue = AllQueues[0];
            
            foreach (var queue in AllQueues)
                if (SqrDistance(queue) < closestDist)
                    closestQueue = queue;

            return closestQueue;

            float SqrDistance(Queue queue) => (queue.transform.position - closeTo).sqrMagnitude;
        }

        public Vector3 GetSpotInLine(int index) => transform.position - spotSpacing * index * transform.forward;

        public bool TryLineUp(out Vector3 spotInline)
        {
            spotInline = Vector3.zero;
            if (NumCustomersInLine >= limit) return false;
            spotInline = GetSpotInLine(NumCustomersInLine);
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
