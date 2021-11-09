using System;
using System.Collections.Generic;
using UnityEngine;

namespace Interactables.Holding
{
    [Serializable]
    public struct HoldableInfo
    {
        public string label;
        
        public bool canBeDropped;
        public bool canBeThrown;
        
        public bool canBeHung;
        public bool groundPlacementOnly;
    }
    
    public static class ItemThumbnail
    {
        public static readonly Vector2Int Dimensions = new(64, 64);
        const bool Orthographic = true;
        
        static readonly Dictionary<string, Texture2D> Thumbnails = new();

        [RuntimeInitializeOnLoadMethod]
        static void InitPreview()
        {
            RuntimePreviewGenerator.MarkTextureNonReadable = false;
            RuntimePreviewGenerator.BackgroundColor = Color.clear;
            RuntimePreviewGenerator.OrthographicMode = Orthographic;
        }

        public static Texture2D Generate(Pickuppable pickuppable)
        {
            var label = pickuppable.Info.label;
            var model = pickuppable.transform;
            
            if (string.IsNullOrEmpty(label))
            {
                Debug.LogWarning("Label was null or empty. Using transform name.");
                label = model.name;
            }

            if (Thumbnails.ContainsKey(label)) return Thumbnails[label];

            var icon = RuntimePreviewGenerator.GenerateModelPreview(model, Dimensions.x, Dimensions.y);
            icon.Compress(true);
            icon.name = label;
            return Thumbnails[label] = icon;
        }
    }
}
