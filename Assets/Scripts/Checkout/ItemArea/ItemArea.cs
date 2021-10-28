using System;
using UnityEngine;

namespace Checkout
{
    public class ItemArea : MonoBehaviour
    {
        [field: SerializeField] public int Width { get; private set; }
        [field: SerializeField] public int Length { get; private set; }
        
        bool[,] occupied;
        
        public const float UnitSize = 0.1f;
        
        void Awake() => occupied = new bool[Width, Length];
        public bool this[int x, int y] => occupied[x, y];
    	
    	public bool TryOccupy(int sizeX, int sizeY, out Vector3 position)
        {
            if (sizeX <= 0 || sizeY <= 0) throw new ArgumentOutOfRangeException();

            var successful = false;
            var pos = -Vector3.one;

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
                    pos = new Vector3(regionX, 0, regionY);
                    EnumerateSpaces((x, y) => {
                        occupied[regionX + x, regionY + y] = true;
                    }, sizeX, sizeY);
                }
                
    		}, Width - sizeX + 1, Length - sizeY + 1);

            var localGridSpace = pos + new Vector3(sizeX - 1, 0, sizeY - 1) / 2;
            position = transform.position + transform.rotation * localGridSpace * UnitSize;
            return successful;
        }
    
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
