using UnityEngine;

namespace UI.Settings
{
    public class PrefSaver : MonoBehaviour
    {
        public void Save()
        {
            ControlOverrideSaver.SetPref();
            PlayerPrefs.Save();
        }
    }
}
