using DG.Tweening;
using UnityEngine;

namespace Interactables.Holding
{
    public class ContainerPositioner : MonoBehaviour
    {
        [SerializeField, Min(1)] Vector3Int containmentBoxSize = Vector3Int.one;
        [SerializeField, Min(0.01f)] float unitSize = 0.25f;
        [SerializeField] Collider disableCollider;
        [SerializeField, Min(0)] Vector3 positionVariance;
        [SerializeField, Min(0)] Vector3 rotationVariance;
        [SerializeField] float placeTime = 0.2f;
        [SerializeField] float tweenStartHeight = 1;
        [SerializeField] float toStartTime = 0.2f;

        Vector3[] positions;
        
        public int TotalPositions => positions.Length;

        void Reset() => TryGetComponent(out disableCollider);

        void Awake()
        {
            positions = new Vector3[containmentBoxSize.x * containmentBoxSize.y * containmentBoxSize.z];
            var i = 0;
            for (var y = 0; y < containmentBoxSize.y; y++)
            for (var x = 0; x < containmentBoxSize.x; x++)
            for (var z = 0; z < containmentBoxSize.z; z++)
            {
                var purePoint = new Vector3(x, y, z) - (containmentBoxSize - Vector3.one) / 2;
                positions[i] = purePoint * unitSize;
                i++;
            }
        }

        void OnDrawGizmosSelected()
        {
            Awake();

            Gizmos.color = new Color(1, 0.4f, 0);
            foreach (var p in positions)
            {
                var pos = transform.TransformPoint(p);
                Gizmos.DrawSphere(pos, 0.05f);
                Gizmos.DrawWireCube(pos, positionVariance * 2);
            }
        }

        Quaternion GenerateRotation() => Quaternion.Euler(GenerateVarianceVector(rotationVariance));

        Vector3 GenerateVarianceVector(Vector3 amount)
        {
            var variance = Vector3.zero;
            
            for (var i = 0; i < 3; i++)
                variance[i] = Random.Range(-amount[i], amount[i]);
            
            return variance;
        }

        public void PlaceInPosition(Transform item, int index, bool tween = true, bool raiseItemFirst = false, TweenCallback callback = null)
        {
            Physics.IgnoreCollision(item.GetComponent<Collider>(), disableCollider);
            var rb = item.GetComponent<Rigidbody>();
            rb.isKinematic = true;
            item.parent = transform;

            var localPos = positions[index] + GenerateVarianceVector(positionVariance);
            var localRot = GenerateRotation();

            if (tween)
            {
                item.DOKill();
                
                var sequence = DOTween.Sequence();
                if (raiseItemFirst)
                {
                    var tweenStartPos = new Vector3(localPos.x, tweenStartHeight, localPos.z);
                    sequence.Append(item.DOLocalMove(tweenStartPos, toStartTime));
                }
                sequence.Append(item.DOLocalMove(localPos, placeTime));
                sequence.Join(item.DOLocalRotateQuaternion(localRot, placeTime));
                sequence.OnComplete(callback);
            }
            else
            {
                item.localPosition = localPos;
                item.localRotation = localRot;
            }
        }

        public void PlaceInPositions(Transform[] items, int startingIndex, bool tween = true)
        {
            for (var i = 0; i < items.Length; i++)
                PlaceInPosition(items[i], startingIndex + i, tween);
        }

        public void RestoreCollision(Component item) =>
            Physics.IgnoreCollision(item.GetComponent<Collider>(), disableCollider, false);
    }
}
