using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class QuitButton : MonoBehaviour
    {
        Button button;
        void Awake() => button = GetComponent<Button>();
        void OnEnable() => button.onClick.AddListener(Quit);
        void OnDisable() => button.onClick.RemoveListener(Quit);
        void Quit()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
