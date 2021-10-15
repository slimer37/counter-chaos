using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Core
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
        static int baseSceneIndex;

        [RuntimeInitializeOnLoadMethod]
        static void InitBaseScene()
        {
            for (var i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                var scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                var sceneNameStart = scenePath.LastIndexOf("/", StringComparison.Ordinal) + 1;
                var sceneNameEnd = scenePath.LastIndexOf(".", StringComparison.Ordinal);
                var sceneNameLength = sceneNameEnd - sceneNameStart;
                
                if (scenePath.Substring(sceneNameStart, sceneNameLength) == "Base")
                {
                    baseSceneIndex = i;
                    return;
                }
            }
        }

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
            if (i == baseSceneIndex)
                throw new ArgumentException("You cannot load the base scene directly.", nameof(i));
            
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            var shouldLoadBase = i != 0;
            
            if (instance) instance.StartCoroutine(instance.LoadAsync(i, shouldLoadBase));
            else
            {
                DOTween.KillAll();
                Debug.LogWarning("Couldn't find a SceneLoader instance. Loading normally...");
                if (shouldLoadBase)
                    SceneManager.LoadScene(baseSceneIndex);
                SceneManager.LoadScene(i, shouldLoadBase ? LoadSceneMode.Additive : LoadSceneMode.Single);
            }
        }

        IEnumerator LoadAsync(int index, bool withBase)
        {
            captionText.text = "";
            percentageText.text = "0%";
            loadingBar.value = 0;
            
            Time.timeScale = 0;
            canvasGroup.blocksRaycasts = true;
            yield return canvasGroup.DOFade(1, fadeDuration).SetUpdate(true).WaitForCompletion();
            DOTween.KillAll();
            
            if (withBase)
                SceneManager.LoadScene(baseSceneIndex);
            
            var op = SceneManager.LoadSceneAsync(index, withBase ? LoadSceneMode.Additive : LoadSceneMode.Single);
            while (!op.isDone)
            {
                var progress = Mathf.Clamp01(op.progress / 0.9f);
                loadingBar.value = progress;

                percentageText.text = $"{Mathf.RoundToInt(progress * 100)}%";
                captionText.text = captions[Mathf.RoundToInt(captions.Length * progress - 0.5f)];
                yield return null;
            }

            if (withBase)
                SceneManager.SetActiveScene(SceneManager.GetSceneAt(1));
            
            Time.timeScale = 1;
            canvasGroup.blocksRaycasts = false;
            yield return canvasGroup.DOFade(0, fadeDuration).SetUpdate(true).WaitForCompletion();
        }
    }
}
