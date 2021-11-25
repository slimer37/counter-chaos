using UnityEngine;
using UnityEngine.SceneManagement;
using Upgrades;

namespace Tutorial.Setup
{
    public class TutorialSceneSetup : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod]
        static void Init() => SceneManager.sceneLoaded += OnSceneLoaded;

        static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (mode != LoadSceneMode.Additive) return;
            
            if (scene.name.ToLowerInvariant().Contains("tutorial"))
                PrepareTutorialScene();
        }

        static void PrepareTutorialScene()
        {
            Destroy(FindObjectOfType<Laptop>());
        }
    }
}
