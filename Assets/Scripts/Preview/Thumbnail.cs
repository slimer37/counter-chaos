using System.Collections.Generic;
using UnityEngine;

namespace Preview
{
    public static class Thumbnail
    {
        static readonly Vector2Int DefaultDimensions = Vector2Int.one * 128;
        const bool Orthographic = true;
        
        static readonly Dictionary<string, Texture2D> Thumbnails = new();

        [RuntimeInitializeOnLoadMethod]
        static void InitPreview()
        {
            RuntimePreviewGenerator.MarkTextureNonReadable = false;
            RuntimePreviewGenerator.BackgroundColor = Color.clear;
            RuntimePreviewGenerator.OrthographicMode = Orthographic;
            RuntimePreviewGenerator.PreviewDirection = new Vector3(-0.5f, -0.5f, -1);
            RuntimePreviewGenerator.Padding = 0.01f;

            var light = new GameObject("Preview Generator Light").AddComponent<Light>();
            light.type = LightType.Spot;
            light.intensity = 1.5f;
            light.spotAngle = 150;
            light.innerSpotAngle = 140;
            light.transform.localPosition = Vector3.back * 0.5f;
            RuntimePreviewGenerator.InternalLight = light;
        }
        
        /// <summary>
        /// Overload for Grab() that uses the default size. See
        /// <see cref="Grab(string,UnityEngine.Transform,UnityEngine.Vector2Int)"/>
        /// </summary>
        public static Texture2D Grab(string key, Transform obj) => Grab(key, obj, DefaultDimensions);

        /// <summary>
        /// Creates a new object thumbnail.
        /// </summary>
        /// <param name="key">If null or empty, the cache will not be used.
        /// Otherwise, if a thumbnail has already been created with this key, it will be returned.</param>
        /// <param name="obj">The subject of the thumbnail.</param>
        /// <param name="size">The thumbnail's dimensions.</param>
        /// <returns>The generated or cached thumbnail.</returns>
        public static Texture2D Grab(string key, Transform obj, Vector2Int size)
        {
            var dontUseCache = string.IsNullOrEmpty(key);
            
            if (!dontUseCache && Thumbnails.ContainsKey(key)) return Thumbnails[key];

            var icon = RuntimePreviewGenerator.GenerateModelPreview(obj, size.x, size.y);
            icon.Compress(true);
            icon.filterMode = FilterMode.Bilinear;
            
            icon.name = string.IsNullOrEmpty(key) ? "Unnamed" : key;

            if (!dontUseCache) Thumbnails[key] = icon;
            
            return icon;
        }
    }
}
