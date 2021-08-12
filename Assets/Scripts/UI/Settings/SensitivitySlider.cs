using UnityEngine;
using UnityEngine.UI;

namespace UI.Settings
{
    public class SensitivitySlider : MonoBehaviour
    {
        [SerializeField] Slider slider;
        
        PlayerController controller;
        
        void Awake()
        {
            var sensitivitySetting = PlayerPrefs.GetFloat("Sensitivity", 80);
            slider.onValueChanged.AddListener(ChangeSensitivity);
            slider.value = sensitivitySetting;
            
            controller = FindObjectOfType<PlayerController>();
            if (!controller) return;
            controller.sensitivity = sensitivitySetting;
        }

        void ChangeSensitivity(float value)
        {
            PlayerPrefs.SetFloat("Sensitivity", value);
            if (!controller) return;
            controller.sensitivity = value;
        }
    }
}
