using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UI
{
    public sealed class Menu : MonoBehaviour
    {
        [SerializeField] InputProvider input;
        
        static readonly List<Menu> OpenMenus = new();

        void Awake() => input.Exit += OnCancel;

        void OnDestroy() => input.Exit -= OnCancel;

        bool IsOnTop() => OpenMenus.Count > 0 && OpenMenus.Last() == this;

        void OnCancel()
        {
            if (!gameObject.activeSelf || !IsOnTop()) return;
            gameObject.SetActive(false);
        }

        void OnEnable() => OpenMenus.Add(this);
        void OnDisable() => OpenMenus.Remove(this);
    }
}
