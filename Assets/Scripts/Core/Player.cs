using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public static class Player
    {
        public static Camera Camera => camera ??= Camera.main;
        static Camera camera;

        [RuntimeInitializeOnLoadMethod]
        static void Attach() => SceneManager.sceneLoaded += (s, l) => camera = null;
    }
}