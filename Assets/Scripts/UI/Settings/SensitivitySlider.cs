using UnityEngine;
using UnityEngine.UI;

namespace UI.Settings
{
    public class SensitivitySlider : MonoBehaviour
    {
        [SerializeField] Slider slider;
        [SerializeField] PlayerController controller;
        
        void Awake()
        {
            controller.sensitivity = PlayerPrefs.GetFloat("Sensitivity", controller.sensitivity);
            slider.value = controller.sensitivity;
            slider.onValueChanged.AddListener(ChangeSensitivity);
        }

        void ChangeSensitivity(float value)
        {
            PlayerPrefs.SetFloat("Sensitivity", value);
            controller.sensitivity = value;
        }
    }
}
