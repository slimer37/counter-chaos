using System.Collections;
using DG.Tweening;
using Interactables.Holding;
using Products;
using UnityEngine;
using UnityEngine.AI;
using Queue = Checkout.Queue;

namespace Customers
{
    public class CustomerBrain : MonoBehaviour
    {
        [SerializeField] NavMeshAgent agent;
        [SerializeField] float rotationSpeed;
        [SerializeField] Animator animator;
        [SerializeField] CustomerHold holder;

        Queue queue;
        bool finishedTransaction;
        ProductInfo requestedProduct;

        int index;
        
        static readonly int Speed = Animator.StringToHash("speed");

        void OnDestroy() => StopAllCoroutines();

        void Update() => animator.SetFloat(Speed, agent.velocity.sqrMagnitude);

        IEnumerator Start()
        {
            var target = ProductManager.GetRandomProductInstance();
            requestedProduct = target.productInfo;
            yield return MoveToward(target.transform.position);
            yield return holder.Pickup(target.GetComponent<Pickuppable>());
            
            queue = Queue.FindClosestQueue(transform.position);
            if (!queue.TryLineUp(out var linePos)) yield break;
            index = queue.NumCustomersInLine - 1;
            yield return MoveToward(linePos);
            
            holder.PrepareToDrop(queue.ItemDropZone);
            yield return MoveToward(queue.ItemDropStandPos);
            yield return Rotate(queue.transform.rotation);
            holder.Drop(queue.ItemDropZone);
            yield return MoveToward(linePos);
            yield return Rotate(queue.transform.rotation);
            
            queue.OnCustomerServed += OnServed;
        }

        void OnServed()
        {
            if (index == 0) StartCoroutine(Finish());
            else
            {
                index--;
                StartCoroutine(MoveToward(queue.GetSpotInLine(index)));
            }
        }

        IEnumerator Finish()
        {
            finishedTransaction = true;
            queue.OnCustomerServed -= OnServed;
            yield return new WaitUntil(() => holder.IsHoldingItem);
            yield return MoveToward(Level.GetFinishPoint());
            Destroy(gameObject);
        }

        IEnumerator MoveToward(Vector3 position)
        {
            agent.SetDestination(position);
            yield return null;
            yield return new WaitUntil(() => agent.remainingDistance < agent.stoppingDistance);
        }

        IEnumerator Rotate(Quaternion rotation)
        {
            yield return transform.DORotateQuaternion(rotation, rotationSpeed).SetSpeedBased().WaitForCompletion();
        }

        void OnCollisionEnter(Collision other)
        {
            if (!holder.IsHoldingItem
                && finishedTransaction 
                && other.transform.CompareTag("Product") 
                && ProductManager.TryGetProductInfo(other.transform, out var info)
                && requestedProduct == info)
                holder.Pickup(other.transform.GetComponent<Pickuppable>());
        }
    }
}
