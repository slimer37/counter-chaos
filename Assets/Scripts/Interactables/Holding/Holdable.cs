using System;

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
}
