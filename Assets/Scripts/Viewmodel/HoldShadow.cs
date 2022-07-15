using Interactables.Holding;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Viewmodel
{
    public class HoldShadow : MonoBehaviour
    {
        [SerializeField] DecalProjector projector;
        [SerializeField] float maxDepth;
        [SerializeField, Tooltip("How much to avoid shadowing the item.")] float exclusion;
        [SerializeField, Tooltip("How much to overlap the projector on a surface.")] float overlap;
        [SerializeField] float shadowSizeMultiplier = 1;
        [SerializeField] ItemHolder holder;
        [SerializeField] GameObject ghost;

        Vector3 GetItemBottom()
        {
            var item = holder.HeldItem;
            return item.transform.position - Vector3.up * item.StandingDistance;
        }

        void SetupNewItem()
        {
            var item = holder.HeldItem;
            var temp = projector.size;
            temp.x = temp.y = item.BoundHalfDiagonal * shadowSizeMultiplier;
            projector.size = temp;
        }
        
        void LateUpdate()
        {
            var itemIsOut = holder.IsDroppingItem && !ghost.activeSelf;

            if (!itemIsOut || 
                !Physics.Raycast(GetItemBottom(), Vector3.down, out var hit, maxDepth))
            {
                projector.enabled = false;
                return;
            }
            
            // First-time setup
            if (!projector.enabled)
            {
                projector.enabled = true;
                SetupNewItem();
            }
            
            transform.position = GetItemBottom() + exclusion *
                (hit.distance > exclusion ? -1 : 1) * Vector3.up;

            var temp = projector.size;
            temp.z = hit.distance + overlap;
            projector.size = temp;

            temp = projector.pivot;
            temp.z = hit.distance / 2;
            projector.pivot = temp;
        }
    }
}
