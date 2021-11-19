using DG.Tweening;
using UnityEngine;

namespace Doors
{
    public class SlidingDoor : MonoBehaviour
    {
        [Header("Trigger Area")]
        [SerializeField] Vector3 areaCenter;
        [SerializeField, Min(0.01f)] Vector3 areaExtents;
        [SerializeField] LayerMask mask;
        
        [Header("Animation")]
        [SerializeField] Vector3 moveAmount;
        [SerializeField] float moveTime;
        [SerializeField] Ease ease;
        [SerializeField] Transform leftDoor;
        
        [Header("Optional")]
        [SerializeField] Transform rightDoor;
        [SerializeField] bool dontNegateMovement;

        Sequence openSequence;
        Vector3 RightDoorMoveAmount => moveAmount * (dontNegateMovement ? 1 : -1);
        
        void OnDrawGizmosSelected()
        {
            var col = Color.yellow;
            col.a = 0.5f;
            Gizmos.color = col;
            
            DrawDoorGizmo(leftDoor, false);
            DrawDoorGizmo(rightDoor, !dontNegateMovement);

            Gizmos.color = Color.green;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(areaCenter, Divide(areaExtents * 2, transform.lossyScale));

            Vector3 Divide(Vector3 a, Vector3 b)
            {
                for (var i = 0; i < 3; i++) a[i] /= b[i];
                return a;
            }
            
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

        void Awake()
        {
            openSequence = DOTween.Sequence();
            openSequence.Append(leftDoor.DOLocalMove(moveAmount, moveTime).SetRelative().SetEase(Ease.Linear));
            if (rightDoor) openSequence.Join(rightDoor.DOLocalMove(RightDoorMoveAmount, moveTime).SetRelative().SetEase(Ease.Linear));
            openSequence.SetAutoKill(false).SetEase(ease).Pause();
            
            // Act like the door just closed so Open doesn't return early
            openSequence.isBackwards = true;
        }

        void FixedUpdate()
        {
            if (Physics.CheckBox(transform.TransformPoint(areaCenter), areaExtents, transform.rotation, mask)) Open();
            else Close();
        }

        void Open()
        {
            if (!openSequence.isBackwards) return;
            openSequence.PlayForward();
        }

        void Close()
        {
            if (openSequence.isBackwards) return;
            openSequence.PlayBackwards();
        }
    }
}
