using System.Linq;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace Project.Editor
{
    public static class BaseSceneLoader
    {
        static readonly int[] ExcludedScenes = {0, 1};

        const string MenuName = "Tools/Load Base Scene";
        const string SettingName = "LoadBaseScene";
        
        static bool IsEnabled
        {
            get => EditorPrefs.GetBool(SettingName, true);
            set => EditorPrefs.SetBool(SettingName, value);
        }
          
        [MenuItem(MenuName)]
        static void Toggle() => IsEnabled = !IsEnabled;

        [MenuItem(MenuName, true)]
        static bool ToggleValidate()
        {
            Menu.SetChecked(MenuName, IsEnabled);
            return true;
        }
        
        [InitializeOnLoadMethod]
        static void Init()
        {
            SceneManager.sceneLoaded += (scene, _) =>
            {
                if (!IsEnabled) return;
                
                if (SceneManager.sceneCount == 1 && !ExcludedScenes.Contains(scene.buildIndex))
                    SceneManager.LoadScene("Base", LoadSceneMode.Additive);
            };
        }
    }
}
