using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.Settings.PlayerModifiers
{
    public class SensitivitySlider : MonoBehaviour
    {
        [SerializeField] Slider slider;
        [SerializeField] PlayerController controller;
        
        void Awake()
        {
            slider.value = PlayerPrefs.GetFloat(PlayerController.SensitivityPrefKey, PlayerController.DefaultSensitivity);
            slider.onValueChanged.AddListener(ChangeSensitivity);
        }

        void ChangeSensitivity(float value)
        {
            PlayerPrefs.SetFloat(PlayerController.SensitivityPrefKey, value);
            if (!controller) return;
            controller.sensitivity = value;
        }
    }
}
