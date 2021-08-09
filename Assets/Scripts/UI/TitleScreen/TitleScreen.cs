using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.TitleScreen
{
    public class TitleScreen : MonoBehaviour
    {
        public void LoadScene(int index) => SceneManager.LoadScene(index);
    }
}
