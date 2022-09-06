using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Settings.PlayerModifiers
{
    public class SensitivitySlider : MonoBehaviour
    {
        [SerializeField] Slider slider;
        [SerializeField] TMP_InputField numInput;
        [SerializeField] PlayerController controller;
        
        void Awake()
        {
            var value = PlayerPrefs.GetFloat(PlayerController.SensitivityPrefKey, PlayerController.DefaultSensitivity);
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
            
            PlayerPrefs.SetFloat(PlayerController.SensitivityPrefKey, value);
            
            if (!controller) return;
            controller.sensitivity = value;
        }

        void UpdateUI(float value)
        {
            slider.value = value;
            numInput.text = slider.value.ToString("F1");
        }
    }
}
