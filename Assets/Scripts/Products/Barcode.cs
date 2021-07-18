using UnityEngine;

namespace Products
{
    public static class Barcode
    {
        const int BarcodeWidth = 50;
        
        static readonly Color SpaceColor = Color.clear;
        static readonly Color BarColor = Color.white;
        const int NarrowWidth = 1;
        const int WideWidth = 2;

        public static Texture2D Generate(int seed)
        {
            var tempState = Random.state;
            Random.InitState(seed);
            
            var barcodeTex = new Texture2D(BarcodeWidth, 1) {filterMode = FilterMode.Point};

            var onBar = true;
            var pixels = new Color[BarcodeWidth];

            for (var i = 0; i < BarcodeWidth;)
            {
                var barWidth = Random.value > 0.5f ? NarrowWidth : WideWidth;

                if (i + barWidth > BarcodeWidth)
                    break;
            
                for (var x = i; x < i + barWidth; x++)
                    pixels[x] = onBar ? BarColor : SpaceColor;
            
                i += barWidth;
                onBar = !onBar;
            }
        
            barcodeTex.SetPixels(pixels);
            barcodeTex.Apply();
            
            // Revert RNG state.
            Random.state = tempState;

            return barcodeTex;
        }
    }
}
