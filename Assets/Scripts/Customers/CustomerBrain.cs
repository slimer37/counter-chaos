using System.Collections;
using Interactables.Holding;
using UnityEngine;
using UnityEngine.AI;

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
        }

        IEnumerator MoveToward(Vector3 position)
        {
            agent.SetDestination(position);
            yield return null;
            yield return new WaitUntil(() => agent.remainingDistance < agent.stoppingDistance);
        }
    }
}
