using Core;
using DG.Tweening;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] CanvasGroup fadeGroup;
    [SerializeField] float fadeTime;
    
    Controls controls;

    bool paused;

    CursorLockMode tempLockMode;
    bool tempCursorVisibility;
    
    void Awake()
    {
        controls = new Controls();
        controls.Menu.Exit.performed += _ => TogglePause();
        controls.Enable();
        
        fadeGroup.gameObject.SetActive(true);
        fadeGroup.alpha = 0;
        fadeGroup.blocksRaycasts = false;
    }

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
        }
        
        Cursor.lockState = paused ? CursorLockMode.None : tempLockMode;
        Cursor.visible = paused || tempCursorVisibility;
    }
}
