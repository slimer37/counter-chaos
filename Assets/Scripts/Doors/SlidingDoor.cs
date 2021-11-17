using DG.Tweening;
using UnityEngine;

namespace Doors
{
    [RequireComponent(typeof(Collider))]
    public class SlidingDoor : MonoBehaviour
    {
        [SerializeField] Vector3 moveAmount;
        [SerializeField] float moveTime;
        [SerializeField] Ease ease;
        [SerializeField] Transform rightDoor;
        
        [Header("Optional")]
        [SerializeField] Transform leftDoor;

        Sequence openSequence;
        
        void OnDrawGizmosSelected()
        {
            DrawDoorGizmo(rightDoor, false);
            DrawDoorGizmo(leftDoor, true);
            
            void DrawDoorGizmo(Transform door, bool negateMove)
            {
                var moveAmt = moveAmount * (negateMove ? -1 : 1);
                
                if (door)
                    Gizmos.DrawWireMesh(
                        door.GetComponent<MeshFilter>().sharedMesh,
                        door.position + door.TransformDirection(moveAmt),
                        door.rotation, door.lossyScale);
            }
        }

        void OnValidate()
        {
            if (!GetComponent<Collider>().isTrigger)
                Debug.LogWarning("Associated sliding door collider should be a trigger.");
        }

        void Awake()
        {
            openSequence = DOTween.Sequence();
            openSequence.Append(rightDoor.DOLocalMove(moveAmount, moveTime).SetRelative().SetEase(Ease.Linear));
            if (leftDoor) openSequence.Join(leftDoor.DOLocalMove(-moveAmount, moveTime).SetRelative().SetEase(Ease.Linear));
            openSequence.SetAutoKill(false).SetEase(ease).Pause();
            
            // Act like the door just closed so OnTriggerStay doesn't return early
            openSequence.isBackwards = true;
        }

        void OnTriggerStay(Collider other)
        {
            if (!openSequence.isBackwards) return;
            openSequence.PlayForward();
        }

        void OnTriggerExit(Collider other)
        {
            if (openSequence.isBackwards) return;
            openSequence.PlayBackwards();
        }
    }
}
