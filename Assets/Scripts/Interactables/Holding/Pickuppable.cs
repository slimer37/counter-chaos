using System;
using UnityEngine;

namespace Interactables.Holding
{
    public sealed class Pickuppable : MonoBehaviour, IInteractable
    {
        [SerializeField] Hoverable hoverable;
        [SerializeField] Rigidbody rb;

        bool isHeld;

        public static event Action<Pickuppable> ItemPickedUp;

        static bool playerIsHoldingObject;
        
        public void OnInteract(Transform player)
        {
            if (playerIsHoldingObject || isHeld) return;

            hoverable.OnHoverExit();
            Setup(player);
            ItemPickedUp?.Invoke(this);
        }

        public void Drop() => Setup(null);

        void Setup(Transform holder)
        {
            transform.parent = holder;
            isHeld = holder;
            rb.isKinematic = holder;
            playerIsHoldingObject = holder;
            hoverable.enabled = !holder;
        }
    }
}
