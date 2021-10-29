using System;
using System.Collections.Generic;
using UnityEngine;

namespace Checkout
{
    public class Queue : MonoBehaviour
    {
        [SerializeField, Min(1)] int limit = 1;
        [SerializeField] float spotSpacing;
        [field: SerializeField] public ItemArea Area { get; private set; }

        public Vector3[] LineSpots { get; private set; }

        public int NumCustomersInLine { get; private set; }

        public event Action OnCustomerServed;
        public event EventHandler<LineMoveEventArgs> MoveLine;

        static readonly List<Queue> AllQueues = new();

        public class LineMoveEventArgs : EventArgs
        {
            public int IndexMoved { get; }

            public LineMoveEventArgs(int indexMoved) => IndexMoved = indexMoved;
        }

        void OnEnable() => AllQueues.Add(this);
        void OnDisable() => AllQueues.Remove(this);

        void Awake() => LineSpots = QueuePositioning.GenerateQueue(transform.position, transform.forward, spotSpacing, limit);

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

        public bool TryLineUp(out Vector3 spotInline)
        {
            spotInline = Vector3.zero;
            if (NumCustomersInLine >= limit) return false;
            spotInline = LineSpots[NumCustomersInLine];
            NumCustomersInLine++;
            return true;
        }

        public void CustomerLeave(int index)
        {
            MoveLine?.Invoke(this, new LineMoveEventArgs(index));
            NumCustomersInLine--;
        }

        public void ServeCustomer()
        {
            if (NumCustomersInLine == 0) throw new InvalidOperationException("ServeCustomer called with no customers in queue.");
            OnCustomerServed?.Invoke();
        }
    }
}
