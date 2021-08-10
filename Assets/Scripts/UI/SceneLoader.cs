using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] float fadeDuration;
        [SerializeField] TextMeshProUGUI percentageText;
        [SerializeField] Slider loadingBar;
        [SerializeField] TextMeshProUGUI captionText;
        [SerializeField] string[] captions;

        static SceneLoader instance;

        void Awake()
        {
            if (instance)
            {
                Destroy(gameObject);
                return;
            }
            
            instance = this;
            canvasGroup.gameObject.SetActive(true);
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0;
            DontDestroyOnLoad(gameObject);
        }

        public static void Load(int i)
        {
            DOTween.KillAll();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            
            if (instance) instance.StartCoroutine(instance.LoadAsync(i));
            else
            {
                Debug.LogWarning("Couldn't find a SceneLoader instance. Loading normally...");
                SceneManager.LoadScene(i);
            }
        }

        IEnumerator LoadAsync(int index)
        {
            Time.timeScale = 0;
            canvasGroup.blocksRaycasts = true;
            yield return canvasGroup.DOFade(1, fadeDuration).SetUpdate(true).WaitForCompletion();
            
            var op = SceneManager.LoadSceneAsync(index);
            while (!op.isDone)
            {
                var progress = Mathf.Clamp01(op.progress / 0.9f);
                loadingBar.value = progress;

                percentageText.text = $"{Mathf.RoundToInt(progress * 100)}%";
                captionText.text = captions[Mathf.RoundToInt(captions.Length * progress - 0.5f)];
                yield return null;
            }
            
            Time.timeScale = 1;
            canvasGroup.blocksRaycasts = false;
            yield return canvasGroup.DOFade(0, fadeDuration).WaitForCompletion();
        }
    }
}
