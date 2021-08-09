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
            instance = this;
            canvasGroup.gameObject.SetActive(true);
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0;
            DontDestroyOnLoad(gameObject);
        }

        public static void Load(int i)
        {
            if (instance) instance.StartCoroutine(instance.LoadAsync(i));
            else
            {
                Debug.LogWarning("Couldn't find a SceneLoader instance. Loading normally...");
                SceneManager.LoadScene(i);
            }
        }

        IEnumerator LoadAsync(int index)
        {
            DOTween.KillAll();
            
            canvasGroup.blocksRaycasts = true;
            yield return canvasGroup.DOFade(1, fadeDuration).WaitForCompletion();
            
            var op = SceneManager.LoadSceneAsync(index);
            while (!op.isDone)
            {
                var progress = Mathf.Clamp01(op.progress / 0.9f);
                loadingBar.value = progress;

                percentageText.text = $"{Mathf.RoundToInt(progress * 100)}%";
                captionText.text = captions[Mathf.RoundToInt(captions.Length * progress - 0.5f)];
                yield return null;
            }
            
            canvasGroup.blocksRaycasts = false;
            yield return canvasGroup.DOFade(0, fadeDuration).WaitForCompletion();
        }
    }
}
