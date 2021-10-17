using System;
using Interactables.Base;
using UnityEngine;

namespace Interactables.Holding
{
    [Serializable]
    public struct HoldableInfo
    {
        public bool canBeDropped;
        public bool canBeThrown;
        
        public bool canBeHung;
        public bool groundPlacementOnly;
    }
    
    public interface IHoldable : IInteractHandler
    {
        public string Label { get; }
        public Sprite Icon { get; }
        
        public HoldableInfo Info { get; }
        public void OnPickup();
        public void OnEquip();
        public void OnDequip();
        
        public Vector3? OverridePosition { get; }
        public Vector3? OverrideRotation { get; }
    }
}
