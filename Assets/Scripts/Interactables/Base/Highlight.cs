﻿using System.Linq;
using UnityEngine;

namespace Interactables.Base
{
    [DisallowMultipleComponent]
    public class Highlight : MonoBehaviour
    {
        Renderer[] renderers;
        Material selectedMaterial;
        
        static readonly int ColorID = Shader.PropertyToID("_Color");
        static readonly int FlashSpeedID = Shader.PropertyToID("_FlashSpeed");
        static readonly int MaxDitherID = Shader.PropertyToID("_MaxDither");
        static readonly int StartTimeID = Shader.PropertyToID("_StartTime");

        void Awake()
        {
            renderers = GetComponentsInChildren<Renderer>();
            if (selectedMaterial) return;
            InitMaterial();
            
            selectedMaterial.SetColor(ColorID, Color.yellow);
            selectedMaterial.SetFloat(FlashSpeedID, 4);
            selectedMaterial.SetFloat(MaxDitherID, 0.3f);
        }

        void InitMaterial() => selectedMaterial = new Material(Shader.Find("Shader Graphs/Selected")) { name = "Selected (Instance)" };

        void OnEnable() => SetHighlightEnabled(true);
        void OnDisable() => SetHighlightEnabled(false);

        void SetHighlightEnabled(bool enable)
        {
            foreach (var rend in renderers)
            {
                var materials = rend.sharedMaterials.ToList();

                if (enable)
                {
                    selectedMaterial.SetFloat(StartTimeID, Time.time);
                    materials.Add(selectedMaterial);
                }
                else materials.Remove(selectedMaterial);

                rend.materials = materials.ToArray();
            }
        }

        void OnDestroy() => Destroy(selectedMaterial);
    }
}