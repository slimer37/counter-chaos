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
        [SerializeField] float dampenSpeed;
        [SerializeField] float rotationSpeed;
        [SerializeField] Animator animator;
        [SerializeField] CustomerHold holder;

        Queue queue;
        bool finishedTransaction;
        ProductInfo requestedProduct;
        
        static Transform[] finishPoints;
        static readonly int Speed = Animator.StringToHash("speed");

        [RuntimeInitializeOnLoadMethod]
        static void InitFinishPoints()
        {
            var finishGOs = GameObject.FindGameObjectsWithTag("Finish");
            finishPoints = new Transform[finishGOs.Length];
            for (var i = 0; i < finishGOs.Length; i++)
                finishPoints[i] = finishGOs[i].transform;
        }

        void OnDestroy() => StopAllCoroutines();

        Vector3 PickFinish() => finishPoints[Random.Range(0, finishPoints.Length)].position;

        void Update() => animator.SetFloat(Speed, agent.velocity.magnitude / dampenSpeed);

        IEnumerator Start()
        {
            var target = ProductManager.GetRandomProductInstance();
            requestedProduct = target.productInfo;
            yield return MoveToward(target.transform.position);
            yield return holder.Pickup(target.GetComponent<Pickuppable>());
            
            queue = Queue.FindClosestQueue(transform.position);
            if (!queue.TryLineUp()) yield break;
            yield return MoveToward(queue.transform.position);
            
            holder.PrepareToDrop(queue.ItemDropZone);
            yield return MoveToward(queue.ItemDropStandPos);
            yield return Rotate(queue.transform.rotation);
            holder.Drop(queue.ItemDropZone);
            yield return MoveToward(queue.transform.position);
            yield return Rotate(queue.transform.rotation);
            
            queue.OnCustomerServed += OnServed;
        }

        void OnServed() => StartCoroutine(Finish());

        IEnumerator Finish()
        {
            finishedTransaction = true;
            queue.OnCustomerServed -= OnServed;
            yield return new WaitUntil(() => holder.IsHoldingItem);
            yield return MoveToward(PickFinish());
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
