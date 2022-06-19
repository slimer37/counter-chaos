using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Interactables.Holding;
using UnityEngine;
using UnityEngine.AI;

namespace Npc
{
    [RequireComponent(typeof(NavMeshAgent), typeof(NpcHand))]
    public class NpcBase : MonoBehaviour
    {
        [SerializeField] Animator animator;
        [SerializeField, Min(1)] float rotationSpeed = 100;
        
        NpcHand hand;
        NavMeshAgent agent;

        List<Pickuppable> items = new();
        public bool IsHoldingItem => items.Count > 0;
        public Pickuppable heldItem;
        
        static readonly int Speed = Animator.StringToHash("speed");

        void Reset() => TryGetComponent(out animator);

        void Awake()
        {
            if (!animator) Debug.LogError("No animator assigned.", this);

            TryGetComponent(out hand);
            TryGetComponent(out agent);
        }

        void OnDestroy() => StopAllCoroutines();
        
        void Update() => animator.SetFloat(Speed, agent.desiredVelocity.sqrMagnitude);
        
        protected IEnumerator MoveToward(Vector3 position)
        {
            agent.SetDestination(position);
            yield return null;
            yield return new WaitUntil(() => agent.remainingDistance < agent.stoppingDistance);
        }

        protected YieldInstruction LookAt(Vector3 position)
        {
            var rotation = Quaternion.LookRotation(position - transform.position);
            return transform.DORotateQuaternion(rotation, rotationSpeed).SetSpeedBased().WaitForCompletion();
        }

        protected YieldInstruction Rotate(Quaternion rotation)
        {
            return transform.DORotateQuaternion(rotation, rotationSpeed).SetSpeedBased().WaitForCompletion();
        }

        protected YieldInstruction Pickup(Pickuppable item)
        {
            items.Add(item);
            return hand.Pickup(item);
        }

        protected YieldInstruction Drop(Vector3 pos, Vector3 rot)
        {
            if (!heldItem) throw new Exception("Drop called with no held item.");
            heldItem = items[0];
            items.Remove(heldItem);
            return hand.Drop(heldItem, pos, rot);
        }
    }
}