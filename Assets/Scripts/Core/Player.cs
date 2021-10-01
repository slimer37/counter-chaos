using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public static class Player
    {
        public static Camera Camera => camera ??= Camera.main;
        public static Transform Transform => transform ??= Camera.transform.root;
        
        static Camera camera;
        static Transform transform;

        [RuntimeInitializeOnLoadMethod]
        static void Attach() => SceneManager.sceneLoaded += (s, l) => camera = null;
    }
}