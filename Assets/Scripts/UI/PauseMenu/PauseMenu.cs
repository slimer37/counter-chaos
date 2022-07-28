using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] InputProvider input;
        [SerializeField] CanvasGroup fadeGroup;
        [SerializeField] float fadeTime;
        [SerializeField] UnityEvent onPause;
        [SerializeField] UnityEvent onUnpause;

        bool paused;

        CursorLockMode tempLockMode;
        bool tempCursorVisibility;
    
        void Awake()
        {
            input.Exit += TogglePause;
        
            fadeGroup.gameObject.SetActive(true);
            fadeGroup.alpha = 0;
            fadeGroup.blocksRaycasts = false;
        }

        void OnDestroy() => input.Exit -= TogglePause;

        public void TogglePause()
        {
            paused = !paused;
            Time.timeScale = paused ? 0 : 1;
            fadeGroup.blocksRaycasts = paused;
            fadeGroup.DOComplete();
            fadeGroup.DOFade(paused ? 1 : 0, fadeTime).SetUpdate(true);

            if (paused)
            {
                tempLockMode = Cursor.lockState;
                tempCursorVisibility = Cursor.visible;
                onPause?.Invoke();
            }
            else onUnpause?.Invoke();
        
            Cursor.lockState = paused ? CursorLockMode.None : tempLockMode;
            Cursor.visible = paused || tempCursorVisibility;
        }
    }
}
