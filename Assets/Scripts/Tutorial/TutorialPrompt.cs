using Core;
using UI;
using UnityEngine;
using UnityEngine.Events;

namespace Tutorial
{
    public class TutorialPrompt : MonoBehaviour
    {
        [SerializeField] string promptTitle;
        [SerializeField, TextArea] string promptMessage;
        [SerializeField, TextArea] string promptNewVersionMessage;
        [SerializeField] Color promptColor;
        [SerializeField] int tutorialSceneIndex;
        [SerializeField] int defaultSceneIndex;
        
        static bool PlayerDidLatestTutorial => PlayerPrefs.GetString(Key) == Application.version;
        static bool PlayerDidOldTutorial => PlayerPrefs.HasKey(Key) && PlayerPrefs.GetString(Key) != Application.version;

        const string Key = "TutorialDone";

        // Also use this for manual tutorial access (i.e. from settings)
        public void DoTutorial() => SceneLoader.Load(tutorialSceneIndex);
        void DeclineTutorial() => SceneLoader.Load(defaultSceneIndex);

        public void BeginPrompt()
        {
            if (!PlayerDidLatestTutorial)
            {
                Dialog.Instance.YesNo(promptTitle,
                    PlayerDidOldTutorial ? promptNewVersionMessage : promptMessage, 
                    a => {
                    if (a) DoTutorial();
                    else DeclineTutorial();
                }, promptColor);
            }
            else
                DeclineTutorial();
            
            PlayerPrefs.SetInt(Key, 1);
            PlayerPrefs.Save();
        }
    }
}
