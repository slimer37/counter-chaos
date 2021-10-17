using System;

namespace Interactables.Holding
{
    [Serializable]
    public struct HoldableInfo
    {
        public bool CanBeDropped;
        public bool CanBeThrown;
        
        public bool canBeHung;
        public bool groundPlacementOnly;
    }
}
