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

        Queue queue;
        
        static Transform[] finishPoints;

        [RuntimeInitializeOnLoadMethod]
        static void InitFinishPoints()
        {
            var finishGOs = GameObject.FindGameObjectsWithTag("Finish");
            finishPoints = new Transform[finishGOs.Length];
            for (var i = 0; i < finishGOs.Length; i++)
                finishPoints[i] = finishGOs[i].transform;
        }

        Vector3 PickFinish() => finishPoints[Random.Range(0, finishPoints.Length)].position;

        void Update() => animator.speed = agent.velocity.magnitude / dampenSpeed;

        IEnumerator Start()
        {
            var target = FindObjectOfType<Pickuppable>();
            yield return MoveToward(target.transform.position);
            yield return holder.Pickup(target);
            queue = Queue.FindClosestQueue(transform.position);
            if (!queue.TryLineUp()) yield break;
            yield return MoveToward(queue.transform.position);
            yield return holder.Drop(queue.ItemDropZone);
            queue.OnCustomerServed += BeginLeave;
        }

        void BeginLeave() => StartCoroutine(Leave());

        IEnumerator Leave()
        {
            queue.OnCustomerServed -= BeginLeave;
            yield return MoveToward(PickFinish());
            Destroy(gameObject);
        }

        IEnumerator MoveToward(Vector3 position)
        {
            agent.SetDestination(position);
            yield return null;
            yield return new WaitUntil(() => agent.remainingDistance < agent.stoppingDistance);
        }
    }
}
