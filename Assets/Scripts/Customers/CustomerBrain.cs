using System.Collections;
using DG.Tweening;
using Interactables.Holding;
using UnityEngine;
using UnityEngine.AI;

namespace Customers
{
    public class CustomerBrain : MonoBehaviour
    {
        [SerializeField] NavMeshAgent agent;
        [SerializeField] Vector3 holdingPosition;
        [SerializeField] float holdAnimDuration;
        [SerializeField] float dampenSpeed;
        [SerializeField] Animator animator;

        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0, 1, 0, 0.5f);
            Gizmos.DrawCube(transform.TransformPoint(holdingPosition), Vector3.one * 0.25f);
        }

        void Update() => animator.speed = agent.velocity.magnitude / dampenSpeed;

        IEnumerator Start()
        {
            var target = FindObjectOfType<Pickuppable>();
            yield return MoveToward(target.transform.position);
            target.OnInteract(transform);
            yield return target.transform.DOLocalMove(holdingPosition, holdAnimDuration).WaitForCompletion();
        }

        IEnumerator MoveToward(Vector3 position)
        {
            agent.SetDestination(position);
            yield return null;
            yield return new WaitUntil(() => agent.remainingDistance < agent.stoppingDistance);
        }
    }
}
