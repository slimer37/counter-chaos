using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.Settings
{
    public class SensitivitySlider : MonoBehaviour
    {
        [SerializeField] Slider slider;
        
        static PlayerController controller;

        const float DefaultSensitivity = 80;

        [RuntimeInitializeOnLoadMethod]
        static void Init()
        {
            SceneManager.sceneLoaded += (scene, mode) => {
                controller = FindObjectOfType<PlayerController>();
                if (!controller) return;
                controller.sensitivity = PlayerPrefs.GetFloat("Sensitivity", DefaultSensitivity);
            };
        }
        
        void Awake()
        {
            slider.value = PlayerPrefs.GetFloat("Sensitivity", DefaultSensitivity);
            slider.onValueChanged.AddListener(ChangeSensitivity);
        }

        void ChangeSensitivity(float value)
        {
            PlayerPrefs.SetFloat("Sensitivity", value);
            if (!controller) return;
            controller.sensitivity = value;
        }
    }
}
