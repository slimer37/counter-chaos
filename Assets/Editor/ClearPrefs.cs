using UnityEditor;
using UnityEngine;

namespace Project.Editor
{
    public static class ClearPrefs
    {
        [MenuItem("Tools/Clear Prefs")]
        static void Clear()
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("Deleted preferences.");
        }
    }
}
