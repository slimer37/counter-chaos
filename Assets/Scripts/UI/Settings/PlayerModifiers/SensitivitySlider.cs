using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Settings.PlayerModifiers
{
    public class SensitivitySlider : MonoBehaviour
    {
        [SerializeField] Slider slider;
        [SerializeField] TMP_InputField numInput;

        public static float CurrentValue;
        
        public const string SensitivityPrefKey = "Sensitivity";
        public const float DefaultSensitivity = 40;
        
        [RuntimeInitializeOnLoadMethod]
        static void Init() => CurrentValue = PlayerPrefs.GetFloat(SensitivityPrefKey, DefaultSensitivity);

        void Awake()
        {
            var value = PlayerPrefs.GetFloat(SensitivityPrefKey, DefaultSensitivity);
            UpdateUI(value);
            
            slider.onValueChanged.AddListener(ChangeSensitivity);

            numInput.contentType = TMP_InputField.ContentType.DecimalNumber;
            numInput.onSubmit.AddListener(ChangeSensitivity);
            numInput.onDeselect.AddListener(ChangeSensitivity);
        }

        void ChangeSensitivity(string value) => ChangeSensitivity(float.Parse(value));

        void ChangeSensitivity(float value)
        {
            // Apply slider min and max by assigning it to the slider.
            UpdateUI(value);
            value = slider.value;
            
            PlayerPrefs.SetFloat(SensitivityPrefKey, value);
        }

        void UpdateUI(float value)
        {
            slider.value = value;
            numInput.text = slider.value.ToString("F1");
            CurrentValue = slider.value;
        }
    }
}
