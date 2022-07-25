using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.ColorPicker
{
    public class ColorPicker : MonoBehaviour, IDragHandler
    {
        [Header("Background")]
        [SerializeField] ComputeShader computeShader;
        [SerializeField] RawImage bgImage;
        [SerializeField, Min(1)] Vector2Int bgResolution = Vector2Int.one * 256;
        
        [Header("Picker")]
        [SerializeField] Transform picker;
        [SerializeField] Image pickerSprite;
        [SerializeField] Image previewSprite;
        
        [Header("Slider")]
        [SerializeField] Slider hueSlider;
        [SerializeField] Image sliderBar;
        [SerializeField, Min(1)] int barResolution = 256;

        public Color OutputColor { get; private set; }
        
        RenderTexture renderTexture;
        Texture2D pickerBackground;
        RectTransform rectT;

        void Awake()
        {
            rectT = bgImage.GetComponent<RectTransform>();
            hueSlider.onValueChanged.AddListener(UpdateHue);
            
            GenerateColorBar();
            ConfigureShader();
        }

        IEnumerator Start()
        {
            UpdateHue(hueSlider.value);
            yield return null;
            UpdateColor();
        }

        void UpdateHue(float value)
        {
            // Run shader
            computeShader.SetFloat("Hue", hueSlider.value);
            computeShader.Dispatch(0,
                bgResolution.x / 8,
                bgResolution.y / 8,
                1);

            RenderTexture.active = renderTexture;
            pickerBackground.ReadPixels(new Rect(Vector2.zero, bgResolution), 0, 0);
            pickerBackground.Apply();
            
            UpdateColor();
        }

        public void OnDrag(PointerEventData eventData)
        {
            picker.position = eventData.position;
            var r = rectT.rect;
            var local = picker.localPosition;
            
            picker.localPosition = new Vector2(
                Mathf.Clamp(local.x, r.xMin, r.xMax),
                Mathf.Clamp(local.y, r.yMin, r.yMax)
            );
            
            UpdateColor();
        }

        void UpdateColor()
        {
            var pickPoint = Rect.PointToNormalized(rectT.rect, picker.localPosition);
            OutputColor = pickerBackground.GetPixel(Mathf.FloorToInt(pickPoint.x * bgResolution.x),
                Mathf.FloorToInt(pickPoint.y * bgResolution.y));
            pickerSprite.color = OutputColor;
            previewSprite.color = OutputColor;
        }

        void GenerateColorBar()
        {
            var tex = new Texture2D(barResolution, 1);
            var colors = new Color[barResolution];
            for (var i = 0; i < colors.Length; i++)
            {
                colors[i] = Color.HSVToRGB((float)i / barResolution, 1, 1);
            }
            tex.SetPixels(colors);
            tex.Apply();
            
            sliderBar.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one / 2);
        }

        void ConfigureShader()
        {
            renderTexture = new RenderTexture(bgResolution.x, bgResolution.y, 24);
            renderTexture.enableRandomWrite = true;
            renderTexture.Create();
            
            pickerBackground = new Texture2D(bgResolution.x, bgResolution.y);
            pickerBackground.wrapMode = TextureWrapMode.Clamp;
            bgImage.texture = pickerBackground;
            
            computeShader.SetTexture(0, "Result", renderTexture);
            computeShader.SetFloats("Dimensions", bgResolution.x, bgResolution.y);
        }
    }
}
