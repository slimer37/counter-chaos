using System.Collections;
using Core;
using UnityEngine;

namespace Tutorial
{
    public abstract class Tutorial : MonoBehaviour
    {
        protected Controls Controls => controls ??= new Controls();
        Controls controls;
        
        public abstract IEnumerator OnTutorialStart();
    }
}
