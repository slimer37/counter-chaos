using TMPro;
using UnityEngine;

namespace UI.TitleScreen
{
    [ExecuteInEditMode, RequireComponent(typeof(TextMeshProUGUI))]
    public class Version : MonoBehaviour
    {
        void Awake() => GetComponent<TextMeshProUGUI>().text = Application.version;
        
        #if UNITY_EDITOR
        void Update()
        {
            if (Application.isPlaying) return;
            Awake();
        }
        #endif
    }
}
