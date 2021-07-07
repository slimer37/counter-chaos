using System;
using UnityEngine;

namespace Core
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        static bool shuttingDown;
        static object lockObj = new object();
        static T instance;

        public static T Instance
        {
            get
            {
                if (shuttingDown)
                {
                    Debug.LogWarning("Singleton " + typeof(T) + " already destroyed. Returning null.");
                    return null;
                }

                lock (lockObj)
                {
                    if (!instance)
                    {
                        instance = FindObjectOfType<T>();
					
                        if (!instance)
                            throw new Exception("Couldn't find any " + typeof(T) + " objects.");
                    }

                    return instance;
                }
            }
        }

        void OnApplicationQuit() => shuttingDown = true;
        void OnDestroy() => shuttingDown = true;
    }
}
