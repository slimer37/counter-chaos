using System.Linq;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace Project.Editor
{
    public static class BaseSceneLoader
    {
        static readonly int[] ExcludedScenes = {0, 1};
        
        [InitializeOnLoadMethod]
        static void Init()
        {
            SceneManager.sceneLoaded += (scene, _) =>
            {
                if (SceneManager.sceneCount == 1 && !ExcludedScenes.Contains(scene.buildIndex))
                    SceneManager.LoadScene("Base", LoadSceneMode.Additive);
            };
        }
    }
}
