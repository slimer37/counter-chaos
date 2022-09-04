using System;
using Interactables.Base;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Interactables.Tools
{
    public abstract class Tool : MonoBehaviour, IInteractable
    {
        [SerializeField] InputActionReference interactAction;

        public static Tool EquippedTool;
        public static event Action<Tool> OnEquipTool; 
        public static event Action<Tool> OnDequipTool; 

        public bool Equipped => EquippedTool == this;
        
        public abstract bool IsCompatibleWith(Toolable toolable);

        public abstract void UseWith(Toolable toolable);

        void OnEnable()
        {
            interactAction.action.performed += OnUse;
        }

        void OnDisable()
        {
            interactAction.action.performed -= OnUse;
        }

        void Update()
        {
            if (!Equipped) return;
            
            if (!interactAction.action.WasPerformedThisFrame())
                OnCarryUpdate();
        }

        protected virtual void OnCarryUpdate() { }
        protected virtual void OnUse(InputAction.CallbackContext ctx) { }

        public void OnInteract(Transform sender)
        {
            EquippedTool = Equipped ? null : this;
            if (Equipped)
            {
                OnEquipTool?.Invoke(this);
                OnEquip(sender);
            }
            else
            {
                OnDequipTool?.Invoke(this);
                OnDequip(sender);
            }
        }

        protected virtual void OnEquip(Transform sender) {}
        protected virtual void OnDequip(Transform sender) {}
    }
}