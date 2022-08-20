using System.Collections;
using System.Collections.Generic;
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
        [SerializeField, Min(0.01f)] float conveyorPlaceAttemptInterval = 1;
        [SerializeField] Animator animator;
        [SerializeField] CustomerHold holder;
        [SerializeField, Min(0)] int minProducts = 3;
        [SerializeField, Min(1)] int maxProducts = 5;

        Queue queue;
        bool finishedTransaction;
        List<ProductIdentifier> requestedProducts = new();

        int index;
        
        static readonly int Speed = Animator.StringToHash("speed");

        void OnDestroy() => StopAllCoroutines();

        void Update() => animator.SetFloat(Speed, agent.velocity.sqrMagnitude);

        IEnumerator Start()
        {
            for (var i = 0; i < Random.Range(minProducts, maxProducts); i++)
            {
                ProductIdentifier newProduct;

                do
                {
                    newProduct = ProductLibrary.GetRandomProductInstance();
                } while (newProduct.GetComponent<Pickuppable>().IsHeld || requestedProducts.Contains(newProduct));
                
                requestedProducts.Add(newProduct);
                
                yield return MoveToward(newProduct.transform.position);

                var pickuppable = newProduct.GetComponent<Pickuppable>();
                if (pickuppable.IsHeld) continue;
                yield return holder.Pickup(pickuppable);
            }
            
            queue = Queue.FindClosestQueue(transform.position);
            if (!queue.TryLineUp(out var linePos)) yield break;
            index = queue.NumCustomersInLine - 1;
            yield return MoveToward(linePos);
            
            if (index > 0)
                yield return Rotate(Quaternion.LookRotation(queue.LineSpots[index - 1] - linePos));

            queue.MoveLine += MoveLine;
            yield return new WaitUntil(() => index == 0);
            queue.MoveLine -= MoveLine;

            foreach (var t in requestedProducts)
            {
                Vector3 position;
                Vector3 rotation;

                while (true)
                {
                    yield return new WaitForSeconds(conveyorPlaceAttemptInterval);
                    if (queue.Area.TryOccupy(t.productInfo.Size, out position, out rotation))
                        break;
                }
                
                queue.Area.StartPlacing();
                yield return holder.Drop(position, rotation);
                queue.Area.EndPlacing();
            }
            
            yield return Rotate(queue.transform.rotation);
            
            queue.OnCustomerServed += OnServed;
        }

        void MoveLine(object sender, Queue.LineMoveEventArgs update)
        {
            if (index > update.IndexMoved)
                StartCoroutine(MoveUpInLine());
        }

        IEnumerator MoveUpInLine()
        {
            yield return MoveToward(queue.LineSpots[index - 1]);
            index--;
        }

        void OnServed()
        {
            StopAllCoroutines();
            StartCoroutine(Finish());
        }

        IEnumerator Finish()
        {
            finishedTransaction = true;
            queue.OnCustomerServed -= OnServed;
            yield return new WaitUntil(() => holder.IsHoldingItem);
            queue.CustomerLeave(index);
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
                && ProductLibrary.TryGetProductInfo(other.transform, out var info)
                && requestedProducts.Find(identifier => identifier.productInfo == info) != null)
                holder.Pickup(other.transform.GetComponent<Pickuppable>());
        }
    }
}
