using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;

namespace Project.Editor
{
    public class UrpMaterialProcessor : AssetPostprocessor
    {
        readonly Dictionary<string, string> floatDefinitions = new()
        {
            { "ReflectionFactor", "_Metallic" },
            { "Shininess", "_Smoothness" }
        };

        readonly Dictionary<string, Func<float, float>> floatConverters = new()
        {
            { "Shininess", f => Mathf.Sqrt(f) / 10 }
        };

        public override int GetPostprocessOrder() => 10;

        void OnPreprocessMaterialDescription(MaterialDescription description, Material material, AnimationClip[] animations)
        {
            foreach (var key in floatDefinitions.Keys)
                if (description.TryGetProperty(key, out float value))
                {
                    if (floatConverters.ContainsKey(key)) value = floatConverters[key](value);
                    material.SetFloat(floatDefinitions[key], value);
                }
        }
        
        void LogProperties(MaterialDescription description)
        {
            var propertyNames = new List<string>();
            description.GetFloatPropertyNames(propertyNames);
            
            for (var i = 0; i < propertyNames.Count; i++)
            {
                description.TryGetProperty(propertyNames[i], out float value);
                propertyNames[i] += " " + value;
            }
            
            Debug.Log(description.materialName + "\n" + string.Join('\n', propertyNames.ToArray()));
        }
    }
}