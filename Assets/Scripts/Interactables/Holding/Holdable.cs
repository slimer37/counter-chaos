using System;
using UnityEngine;

namespace Interactables.Holding
{
    [Serializable]
    public struct HoldableInfo
    {
        public string label;
        
        public bool canBeDropped;
        public bool canBeThrown;
        
        public bool canBeHung;
        public bool groundPlacementOnly;

        [SerializeField] Vector3 overridePosition;
        [SerializeField] bool useRotationIfZero;
        [SerializeField] Vector3 overrideRotation;

        public Vector3? OverridePosition => NullIfZero(overridePosition);
        public Vector3? OverrideRotation => useRotationIfZero ? overrideRotation : NullIfZero(overrideRotation);

        Vector3? NullIfZero(Vector3 v) => v == Vector3.zero ? v : null;
    }
}
