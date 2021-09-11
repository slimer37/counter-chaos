using System.Collections.Generic;
using System.Linq;
using Core;
using UnityEngine;

namespace UI
{
    public sealed class Menu : MonoBehaviour
    {
        Controls controls;
        static readonly List<Menu> OpenMenus = new List<Menu>();

        void Awake()
        {
            controls = new Controls();
            controls.Enable();
            controls.Menu.Exit.performed += _ => OnCancel();
        }

        bool IsOnTop() => OpenMenus.Count > 0 && OpenMenus.Last() == this;

        void OnCancel()
        {
            if (!gameObject.activeSelf || !IsOnTop()) return;
            gameObject.SetActive(false);
        }

        void OnEnable() => OpenMenus.Add(this);
        void OnDisable() => OpenMenus.Remove(this);

        void OnDestroy() => controls.Dispose();
    }
}
