using UnityEngine;
using UnityEngine.UI;

namespace UI.Tabs
{
    public class TabButton : MonoBehaviour
    {
        [SerializeField] TabGroup tabGroup;
        [SerializeField] int tabIndex;

        void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() => tabGroup.SetTab(tabIndex));
        }
    }
}
