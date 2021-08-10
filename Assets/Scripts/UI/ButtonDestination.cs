using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Button))]
    public class ButtonDestination : MonoBehaviour
    {
        [SerializeField] int sceneIndex;
        Button button;
        void Awake() => button = GetComponent<Button>();
        void OnEnable() => button.onClick.AddListener(LoadScene);
        void OnDisable() => button.onClick.RemoveListener(LoadScene);
        void LoadScene() => SceneLoader.Load(sceneIndex);
    }
}
