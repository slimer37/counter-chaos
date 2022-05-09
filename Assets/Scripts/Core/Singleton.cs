using System;
using UnityEngine;

namespace Core
{
    public abstract class SelfStartSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        static T instance;
        
        public static T Instance
        {
            get
            {
                if (instance) return instance;

                instance = new GameObject(nameof(T)).AddComponent<T>();
                DontDestroyOnLoad(instance.gameObject);
                
                return instance;
            }
        }
    }

    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        static T instance;

        public static T Instance
        {
            get
            {
                if (instance) return instance;
                
                instance = FindObjectOfType<T>();
                if (!instance) throw new Exception("Couldn't find any " + typeof(T) + " objects.");

                return instance;
            }
        }
    }
}
