using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.Settings.PlayerModifiers
{
    public class SensitivitySlider : MonoBehaviour
    {
        [SerializeField] Slider slider;
        
        static PlayerController controller;

        #if UNITY_EDITOR
        // Editor sensitivity
        const float DefaultSensitivity = 80;
        #else
        // Release sensitivity
        const float DefaultSensitivity = 40;
        #endif

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
