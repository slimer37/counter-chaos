using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Checkout
{
    public class ItemArea : MonoBehaviour
    {
        [field: Header("Size")]
        [field: SerializeField, Min(1)] public int Width { get; private set; } = 1;
        [field: SerializeField, Min(1)] public int Length { get; private set; } = 1;
        
        [Header("Updating")]
        [SerializeField, Min(0)] float refreshTime;
        [SerializeField, Min(0)] float boxcastDist;
        [SerializeField] LayerMask detectMask;
        
        bool[,] occupied;
        
        // Change display unit in ProductIdentifier when changing this
        public const float UnitSize = 0.1f;
        
        void Awake() => occupied = new bool[Width, Length];
        public bool this[int x, int y] => occupied[x, y];

        void OnDestroy() => StopAllCoroutines();

        IEnumerator Start()
        {
            while (true)
            {
                yield return new WaitForSeconds(refreshTime);
                RecheckOccupiedSpaces();
            }
        }

        public void StartPlacing() => StopAllCoroutines();
        public void EndPlacing() => StartCoroutine(Start());

        void RecheckOccupiedSpaces()
        {
            var results = new RaycastHit[1];
            EnumerateSpaces((x, y) => {
                occupied[x, y] = Physics.BoxCastNonAlloc(GridToWorld(x, y),
                    Vector3.one * UnitSize / 2, Vector3.up, results, transform.rotation, 
                    boxcastDist, detectMask) > 0;
            });
        }
    	
    	public bool TryOccupy(int sizeX, int sizeY, out Vector3 position, out Vector3 rotation)
        {
            var alternate = Random.value > 0.5f;
            if (alternate)
                if (UseAlternate(out position, out rotation))
                    return true;
            
            if (TryOccupy(sizeX, sizeY, out position))
            {
                rotation = transform.eulerAngles;
                return true;
            }

            if (!alternate)
                if (UseAlternate(out position, out rotation))
                    return true;

            rotation = -Vector3.one;

            return false;

            bool UseAlternate(out Vector3 pos, out Vector3 rot)
            {
                var success = TryOccupy(sizeY, sizeX, out pos);
                rot = transform.eulerAngles + Vector3.up * 90;
                return success;
            }
        }

        bool TryOccupy(int sizeX, int sizeY, out Vector3 position)
        {
            if (sizeX <= 0 || sizeY <= 0) throw new ArgumentOutOfRangeException();

            var successful = false;
            var pos = -Vector2Int.one;

            EnumerateSpaces((regionX, regionY) => {
                if (successful) return;
                
                var fail = false;
                
                EnumerateSpaces((x, y) => {
                    if (fail) return;
                    if (occupied[regionX + x, regionY + y]) fail = true;
                }, sizeX, sizeY);

                if (!fail)
                {
                    successful = true;
                    pos = new Vector2Int(regionX, regionY);
                    EnumerateSpaces((x, y) => {
                        occupied[regionX + x, regionY + y] = true;
                    }, sizeX, sizeY);
                }
                
            }, Width - sizeX + 1, Length - sizeY + 1);
            
            // Use Vector2 so half-spaces can be calculated.
            var localGridSpace = pos + new Vector2(sizeX - 1, sizeY - 1) / 2;
            position = GridToWorld(localGridSpace.x, localGridSpace.y);
            return successful;
        }

        Vector3 GridToWorld(float x, float y) =>
            transform.position + transform.rotation * new Vector3(x, 0, y) * UnitSize;

        public void PrintContents()
        {
            var str = "";
    		EnumerateSpaces((x, y) => str += occupied[y, x] ? "[X]" : "[  ]", _ => str += "\n");
            Debug.Log(str);
        }
    	
    	void EnumerateSpaces(Action<int, int> examine, Action<int> passRow = null) => EnumerateSpaces(examine, Width, Length, passRow);
    
    	void EnumerateSpaces(Action<int, int> examine, int xLimit, int yLimit, Action<int> passRow = null)
    	{
    		for (var y = 0; y < yLimit; y++)
    		{
    			for (var x = 0; x < xLimit; x++)
    				examine?.Invoke(x, y);
    			passRow?.Invoke(y);
    		}
    	}
    }
}
