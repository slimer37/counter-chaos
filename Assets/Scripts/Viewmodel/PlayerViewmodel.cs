using Interactables.Holding;
using UnityEngine;

namespace Viewmodel
{
    public class PlayerViewmodel : MonoBehaviour
    {
        [Header("Rig")]
        [SerializeField] Transform target;
        [SerializeField] float offCenterMultiplier = 0.5f;
        [SerializeField] Transform rootBone;
        [SerializeField] float armLength;
        
        [Header("Misc")]
        [SerializeField] Renderer rend;
        [SerializeField] ItemHolder holder;
        [SerializeField] GameObject ghost;

        Vector3 originalLocalPos;

        void Awake()
        {
            originalLocalPos = transform.InverseTransformPoint(rootBone.position);
        }

        void Update()
        {
            rend.enabled = holder.IsDroppingItem && !ghost.activeSelf;
            
            if (!rend.enabled) return;

            var item = holder.HeldItem;
            target.position =
                item.transform.position
                + Vector3.up * item.ToTopDistance
                - item.transform.right * (item.BoundHalfDiagonal * offCenterMultiplier);

            var originalPos = transform.TransformPoint(originalLocalPos);
            var outward = target.position - originalPos;
            rootBone.position = outward.sqrMagnitude > armLength * armLength
                ? target.position - outward.normalized * armLength
                : originalPos;
        }
    }
}
