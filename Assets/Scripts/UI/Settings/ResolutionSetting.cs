using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Settings
{
    public class ResolutionSetting : MonoBehaviour
    {
        [SerializeField] TMP_Dropdown dropdown;
        [SerializeField] Toggle fullscreenToggle;
	
        [Header("Resolution Confirmation")]
        [SerializeField] GameObject resConfirmWindow;
        [SerializeField] TextMeshProUGUI resConfirmText;
        [SerializeField] Button resConfirmButton;
        [SerializeField] int confirmationTime;

        int lastResIndex;
        string formatterText;
        Resolution requestedRes;

        static bool Fullscreen => PlayerPrefs.GetInt("Fullscreen", 1) == 1;

        static Resolution GetResolutionPref() => new()
        {
            width = PlayerPrefs.GetInt("Res X", Screen.currentResolution.width),
            height = PlayerPrefs.GetInt("Res Y", Screen.currentResolution.height),
            refreshRate = PlayerPrefs.GetInt("Refresh Rate", Screen.currentResolution.refreshRate)
        };

        [RuntimeInitializeOnLoadMethod]
        static void SetToPrefResolution()
        {
            var resPref = GetResolutionPref();
            Screen.SetResolution(resPref.width, resPref.height, Fullscreen, resPref.refreshRate);
        }

        void Awake()
        {
            dropdown.options.Clear();
            
            foreach (var resolution in Screen.resolutions)
                dropdown.options.Add(new TMP_Dropdown.OptionData(resolution.ToString()));

            var resPref = GetResolutionPref();
            for (var i = 0; i < Screen.resolutions.Length; i++)
            {
                var resolution = Screen.resolutions[i];
                if (!resPref.Equals(resolution)) continue;
                dropdown.value = i;
                break;
            }
		
            fullscreenToggle.isOn = Fullscreen;
		
            dropdown.onValueChanged.AddListener(OnResolutionSelected);
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggle);
            resConfirmButton.onClick.AddListener(ConfirmResolution);

            formatterText = resConfirmText.text;
            lastResIndex = dropdown.value;
        }

        void OnResolutionSelected(int index)
        {
            if (index == lastResIndex) return;
		
            requestedRes = Screen.resolutions[index];
            Screen.SetResolution(requestedRes.width, requestedRes.height, Fullscreen, requestedRes.refreshRate);

            StartCoroutine(ShowConfirmation());
        }

        void OnFullscreenToggle(bool fullscreen)
        {
            PlayerPrefs.SetInt("Fullscreen", fullscreen ? 1 : 0);
            Screen.fullScreen = fullscreen;
        }

        void ConfirmResolution()
        {
            resConfirmWindow.SetActive(false);
            PlayerPrefs.SetInt("Res X", requestedRes.width);
            PlayerPrefs.SetInt("Res Y", requestedRes.height);
            PlayerPrefs.SetInt("Refresh Rate", requestedRes.refreshRate);
            lastResIndex = dropdown.value;
            StopAllCoroutines();
        }

        IEnumerator ShowConfirmation()
        {
            resConfirmWindow.SetActive(true);

            for (var i = confirmationTime; i > 0; i--)
            {
                resConfirmText.text = string.Format(formatterText, i);
                yield return new WaitForSecondsRealtime(1);
            }
		
            // PlayerPrefs are only updated when confirmed.
            SetToPrefResolution();
		
            resConfirmWindow.SetActive(false);
            dropdown.value = lastResIndex;
        }
    }
}
