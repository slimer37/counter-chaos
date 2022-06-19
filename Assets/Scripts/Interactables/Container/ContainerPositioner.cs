using DG.Tweening;
using Interactables.Base;
using Interactables.Holding;
using UnityEngine;

namespace Interactables.Container
{
    public class ContainerPositioner : MonoBehaviour
    {
        struct Item
        {
            readonly Collider collider;
            public readonly int originalLayer;
            public readonly GameObject gameObject;
            public readonly Pickuppable pickuppable;
            public readonly Hoverable hoverable;

            public Item(GameObject obj)
            {
                originalLayer = obj.layer;
                obj.TryGetComponent(out pickuppable);
                obj.TryGetComponent(out hoverable);
                obj.TryGetComponent(out collider);
                gameObject = obj;
            }

            public void Setup(int layer, Collider ignore) => Setup(layer, ignore, true);
            public void Reset(Collider ignore) => Setup(originalLayer, ignore, false);

            void Setup(int layer, Collider ignore, bool shouldIgnore)
            {
                Physics.IgnoreCollision(collider, ignore, shouldIgnore);
                gameObject.layer = layer;
            }
        }
        
        [SerializeField, Min(1)] Vector3Int containmentBoxSize = Vector3Int.one;
        [SerializeField, Min(0.01f)] float unitSize = 0.25f;
        [SerializeField] Collider disableCollider;
        [SerializeField] Vector3 baseContentsOffset;
        [SerializeField, Min(0)] Vector3 positionVariance;
        [SerializeField, Min(0)] Vector3 baseContentsRotation;
        [SerializeField, Min(0)] Vector3 rotationVariance;
        [SerializeField] float placeTime = 0.2f;
        [SerializeField] float tweenStartHeight = 1;
        [SerializeField] float toStartTime = 0.2f;

        Vector3[] positions;
        Item[] items;
        Sequence currentSequence;
        int ignoreCollisionsLayer;
        
        public int TotalPositions => positions.Length;
        public bool IsAnimating => currentSequence.IsActive() && currentSequence.IsPlaying();

        void Awake()
        {
            var numPoints = containmentBoxSize.x * containmentBoxSize.y * containmentBoxSize.z;
            positions = new Vector3[numPoints];
            items = new Item[numPoints];
            ignoreCollisionsLayer = LayerMask.NameToLayer("Ignore Collision");
            
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
                var pos = transform.TransformPoint(p + baseContentsOffset);
                Gizmos.DrawSphere(pos, 0.05f);
                Gizmos.DrawWireCube(pos, positionVariance * 2);
            }
        }

        Quaternion GenerateRotation() => Quaternion.Euler(baseContentsRotation + GenerateVarianceVector(rotationVariance));

        Vector3 GenerateVarianceVector(Vector3 amount)
        {
            var variance = Vector3.zero;
            
            for (var i = 0; i < 3; i++)
                variance[i] = Random.Range(-amount[i], amount[i]);
            
            return variance;
        }

        public void PlaceInPosition(Transform item, int index, bool tween = true, bool raiseItemFirst = false)
        {
            items[index] = new Item(item.gameObject);
            items[index].Setup(ignoreCollisionsLayer, disableCollider);
            
            var rb = item.GetComponent<Rigidbody>();
            rb.isKinematic = true;
            item.parent = transform;

            var localPos = positions[index] + baseContentsOffset + GenerateVarianceVector(positionVariance);
            var localRot = GenerateRotation();

            if (tween)
            {
                item.DOKill();
                
                currentSequence = DOTween.Sequence();
                if (raiseItemFirst)
                {
                    var tweenStartPos = new Vector3(localPos.x, tweenStartHeight, localPos.z);
                    currentSequence.Append(item.DOLocalMove(tweenStartPos, toStartTime));
                }
                currentSequence.Append(item.DOLocalMove(localPos, placeTime));
                currentSequence.Join(item.DOLocalRotateQuaternion(localRot, placeTime));
            }
            else
            {
                item.localPosition = localPos;
                item.localRotation = localRot;
            }
        }

        public void PlaceInPositions(Transform[] set, int startingIndex, bool tween = true)
        {
            for (var i = 0; i < set.Length; i++)
                PlaceInPosition(set[i], startingIndex + i, tween);
        }

        public bool TryGiveToPlayer(int index)
        {
            var item = items[index];

            if (IsAnimating || Inventory.Main.IsFull) return false;
            
            item.gameObject.transform.DOKill();
            item.Reset(disableCollider);
            
            Inventory.Main.TryGive(item.pickuppable);
            
            item.hoverable.OnHoverExit();

            return true;
        }
    }
}
