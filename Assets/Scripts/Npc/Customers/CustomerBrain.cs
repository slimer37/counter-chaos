using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Interactables.Holding;
using Products;
using UnityEngine;
using Queue = Checkout.Queue;

namespace Npc.Customers
{
    [RequireComponent(typeof(NpcHand))]
    public class CustomerBrain : NpcBase
    {
        [SerializeField, Min(0.01f)] float conveyorPlaceAttemptInterval = 1;
        [SerializeField, Min(0)] int minProducts = 3;
        [SerializeField, Min(1)] int maxProducts = 5;

        Queue queue;
        bool finishedTransaction;     
        
        readonly List<ProductIdentifier> requestedProducts = new();

        int index;

        ProductIdentifier SelectNewProduct()
        {
            ProductIdentifier newProduct;
            do
            {
                newProduct = ProductLibrary.GetRandomProductInstance();
            } while (newProduct.GetComponent<Pickuppable>().IsHeld || requestedProducts.Contains(newProduct));
            
            requestedProducts.Add(newProduct);
            
            return newProduct;
        }

        IEnumerator Start()
        {
            for (var i = 0; i < Random.Range(minProducts, maxProducts); i++)
            {
                var newProduct = SelectNewProduct();
                
                yield return MoveToward(newProduct.transform.position);

                var pickuppable = newProduct.GetComponent<Pickuppable>();
                if (pickuppable.IsHeld) continue;
                yield return Pickup(pickuppable);
            }
            
            queue = Queue.FindClosestQueue(transform.position);
            if (!queue.TryLineUp(out var linePos)) yield break;
            index = queue.NumCustomersInLine - 1;
            yield return MoveToward(linePos);
            
            if (index > 0)
                yield return LookAt(queue.LineSpots[index - 1]);

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
                yield return Drop(position, rotation);
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
            yield return new WaitUntil(() => IsHoldingItem);
            queue.CustomerLeave(index);
            yield return MoveToward(Level.GetFinishPoint());
            Destroy(gameObject);
        }

        void OnCollisionEnter(Collision other)
        {
            if (!IsHoldingItem
                && finishedTransaction 
                && other.transform.CompareTag("Product") 
                && ProductLibrary.TryGetProductInfo(other.transform, out var info)
                && requestedProducts.Find(identifier => identifier.productInfo == info) != null)
                Pickup(other.transform.GetComponent<Pickuppable>());
        }
    }
}
