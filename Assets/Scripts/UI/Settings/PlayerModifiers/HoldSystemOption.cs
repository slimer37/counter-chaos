using Interactables.Holding;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Settings.PlayerModifiers
{
    public class HoldSystemOption : MonoBehaviour
    {
        [SerializeField] ItemHolder holder;
        [SerializeField] Toggle toggle;

        void Awake()
        {
            toggle.isOn = PlayerPrefs.GetInt(ItemHolder.UseOldSystemPrefKey) == 1;
            toggle.onValueChanged.AddListener(OnToggle);
        }

        void OnToggle(bool value)
        {
            PlayerPrefs.SetInt(ItemHolder.UseOldSystemPrefKey, value ? 1 : 0);
            if (holder) holder.useOldSystem = value;
        }
    }
}
