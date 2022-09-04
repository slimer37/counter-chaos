using Core;
using Furniture;
using Interactables.Base;
using JetBrains.Annotations;
using UnityEngine;

namespace Interactables.Tools
{
    public abstract class Toolable : MonoBehaviour, IInteractable
    {
        [SerializeField, Layer] LayerMask toolLayer;
        [SerializeField, Layer] LayerMask defaultLayer;
        
        void Awake()
        {
            Tool.OnEquipTool += t =>
            {
                if (t is FurnitureMover)
                    gameObject.layer = toolLayer;
            };
            
            Tool.OnDequipTool += t =>
            {
                if (t is FurnitureMover)
                    gameObject.layer = defaultLayer;
            };
        }

        [CanBeNull]
        static Tool GetPlayerTool()
        {
            var item = Tool.EquippedTool;
            return item ? item.GetComponent<Tool>() : null;
        }
        
        public bool CanInteract(Transform sender)
        {
            var tool = GetPlayerTool();
            print("can interact");
            return tool && tool.IsCompatibleWith(this);
        }

        public void OnInteract(Transform sender)
        {
            var tool = GetPlayerTool();
            if (!tool) return;
            tool.UseWith(this);
            OnToolUse(tool);
        }

        protected abstract void OnToolUse(Tool tool);
    }
}