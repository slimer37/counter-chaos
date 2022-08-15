using System;
using UnityEngine;

namespace Core
{
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
