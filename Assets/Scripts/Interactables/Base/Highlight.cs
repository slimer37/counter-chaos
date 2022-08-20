using System.Linq;
using UnityEngine;

namespace Interactables.Base
{
    [DisallowMultipleComponent]
    public class Highlight : MonoBehaviour
    {
        Renderer[] renderers;
        Material selectedMaterial;
        
        static readonly int ColorID = Shader.PropertyToID("_Color");

        void Awake()
        {
            renderers = GetComponentsInChildren<Renderer>();
            if (selectedMaterial) return;
            InitMaterial();
        }

        void InitMaterial() => selectedMaterial = new Material(Shader.Find("Shader Graphs/Selected")) { name = "Selected (Instance)" };

        void OnEnable() => SetHighlightEnabled(true);
        void OnDisable() => SetHighlightEnabled(false);

        void SetHighlightEnabled(bool enable)
        {
            foreach (var rend in renderers)
            {
                var materials = rend.sharedMaterials.ToList();

                if (enable) materials.Add(selectedMaterial);
                else materials.Remove(selectedMaterial);

                rend.materials = materials.ToArray();
            }
        }

        void OnDestroy() => Destroy(selectedMaterial);
    }
}
