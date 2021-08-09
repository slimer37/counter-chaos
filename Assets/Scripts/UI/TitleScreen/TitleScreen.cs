using UnityEngine;

namespace UI.TitleScreen
{
    public class TitleScreen : MonoBehaviour
    {
        public void LoadScene(int index) => SceneLoader.Load(index);
    }
}
