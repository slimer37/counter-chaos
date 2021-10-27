using System;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Checkout
{
    public class ItemArea
    {
        public const float UnitSize = 0.1f;
        readonly bool[,] occupied;
    	readonly int width, length;

        [MenuItem("Tools/Test Item Area")]
        static void Test()
        {
            var area = new ItemArea(5, 5);

            bool working;
            do
            {
                var x = Random.Range(1, 4);
                var y = Random.Range(1, 4);
                Debug.Log("adding: " + x + " x " + y);
                
                var col = ColorUtility.ToHtmlStringRGB(Random.ColorHSV(0, 1, 0, 1, 1, 1));
                working = area.TryOccupy(x, y, out _);
                
                if (working) area.PrintContents();
                else Debug.LogWarning("Last failed.");
            } while (working);
        }

        public bool this[int x, int y] => occupied[x, y];
    	
    	public bool TryOccupy(int sizeX, int sizeY, out Vector3 position)
        {
            if (sizeX <= 0 || sizeY <= 0) throw new ArgumentOutOfRangeException();

            var successful = false;
            var pos = new Vector2(-1, -1);

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
                    pos = new Vector2(regionX, regionY);
                    EnumerateSpaces((x, y) => {
                        occupied[regionX + x, regionY + y] = true;
                    }, sizeX, sizeY);
                }
                
    		}, width - sizeX + 1, length - sizeY + 1);

            position = (pos + new Vector2(sizeX, sizeY) / 2) * UnitSize;
            return successful;
        }
    
    	public void PrintContents()
        {
            var str = "";
    		EnumerateSpaces((x, y) => str += occupied[y, x] ? "[X]" : "[  ]", _ => str += "\n");
            Debug.Log(str);
        }
    	
    	void EnumerateSpaces(Action<int, int> examine, Action<int> passRow = null) => EnumerateSpaces(examine, width, length, passRow);
    
    	void EnumerateSpaces(Action<int, int> examine, int xLimit, int yLimit, Action<int> passRow = null)
    	{
    		for (var y = 0; y < xLimit; y++)
    		{
    			for (var x = 0; x < yLimit; x++)
    				examine?.Invoke(x, y);
    			passRow?.Invoke(y);
    		}
    	}
    
    	public ItemArea(int width, int length)
    	{
    		this.width = width;
    		this.length = length;
    		occupied = new bool[width, length];
    	}
    }
}
