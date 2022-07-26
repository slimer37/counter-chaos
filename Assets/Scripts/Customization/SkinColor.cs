using System.Linq;
using UI.ColorPicker;
using UnityEngine;

namespace Customization
{
    public class SkinColor : MonoBehaviour
    {
        [SerializeField] ColorPicker picker;
        [SerializeField] Renderer[] exclude;
        
        Renderer[] bodyRenderers;
        Color currentColor;

        const string PrefKey = "Skin Color";

        void Awake()
        {
            bodyRenderers = GetComponentsInChildren<Renderer>();
            
            var list = bodyRenderers.ToList();
            foreach (var r in exclude)
                list.Remove(r);
            
            bodyRenderers = list.ToArray();
            
            InitColor();
        }
        
        [ContextMenu("Init Color")]
        public void InitColor()
        {
            var defaultColor = bodyRenderers[0].material.color;
            var pref = PlayerPrefs.GetString(PrefKey, "none");
            
            var color = defaultColor;
            
            if (pref != "none")
                ColorUtility.TryParseHtmlString("#" + pref, out color);
            
            SetColor(color);
            picker.Color = color;
        }

        public void Apply()
        {
            PlayerPrefs.SetString(PrefKey, ColorUtility.ToHtmlStringRGB(currentColor));
            PlayerPrefs.Save();
        }

        public void SetColor(Color color)
        {
            currentColor = color;

            foreach (var rend in bodyRenderers)
                rend.material.color = color;
        }
    }
}
