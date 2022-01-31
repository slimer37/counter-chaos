using System.Collections.Generic;
using UnityEngine;

namespace Preview
{
    public static class Thumbnail
    {
        public static readonly Vector2Int Dimensions = Vector2Int.one * 128;
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

        public static Texture2D Grab(string key, Transform obj)
        {
            var model = obj.transform;
            
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogWarning("Key was null or empty. Using transform name.");
                key = model.name;
            }

            if (Thumbnails.ContainsKey(key)) return Thumbnails[key];

            var icon = RuntimePreviewGenerator.GenerateModelPreview(model, Dimensions.x, Dimensions.y);
            icon.Compress(true);
            icon.name = key;
            return Thumbnails[key] = icon;
        }
    }
}
