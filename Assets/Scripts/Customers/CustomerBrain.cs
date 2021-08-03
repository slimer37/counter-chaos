using System.Collections;
using Interactables.Holding;
using UnityEngine;
using UnityEngine.AI;
using Queue = Checkout.Queue;

namespace Customers
{
    public class CustomerBrain : MonoBehaviour
    {
        [SerializeField] NavMeshAgent agent;
        [SerializeField] float dampenSpeed;
        [SerializeField] Animator animator;
        [SerializeField] CustomerHold holder;

        void Update() => animator.speed = agent.velocity.magnitude / dampenSpeed;

        IEnumerator Start()
        {
            var target = FindObjectOfType<Pickuppable>();
            yield return MoveToward(target.transform.position);
            yield return holder.Pickup(target);
            var queue = Queue.FindClosestQueue(transform.position);
            if (!queue.TryLineUp()) yield break;
            yield return MoveToward(queue.transform.position);
            yield return holder.Drop(queue.ItemDropZone);
        }

        IEnumerator MoveToward(Vector3 position)
        {
            agent.SetDestination(position);
            yield return null;
            yield return new WaitUntil(() => agent.remainingDistance < agent.stoppingDistance);
        }
    }
}
