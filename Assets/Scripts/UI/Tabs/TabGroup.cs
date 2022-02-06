using UnityEngine;

namespace UI.Tabs
{
    public class TabGroup : MonoBehaviour
    {
        [SerializeField] GameObject[] tabs;

        void Awake() => SetTab(0);

        internal void SetTab(int i)
        {
            foreach (var tab in tabs)
                tab.SetActive(false);
            
            tabs[i].SetActive(true);
        }
    }
}
