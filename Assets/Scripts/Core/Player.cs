using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public static class Player
    {
        public static Camera Camera
        {
            get
            {
                if (!camera)
                    camera = Camera.main;
                
                return camera;
            }
        }
        
        public static Transform Transform
        {
            get
            {
                if (!transform)
                    transform = Camera.transform.root;

                return transform;
            }
        }
        
        static Camera camera;
        static Transform transform;

        [RuntimeInitializeOnLoadMethod]
        static void Attach() => SceneManager.sceneLoaded += (s, l) => camera = null;
    }
}