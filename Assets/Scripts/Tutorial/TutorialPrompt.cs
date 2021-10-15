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
        [SerializeField] Color promptColor;
        [SerializeField] int tutorialSceneIndex;
        [SerializeField] int defaultSceneIndex;
        
        static bool PlayerDidTutorial => PlayerPrefs.GetInt(Key, 0) == 1;

        const string Key = "TutorialDone";

        // Also use this for manual tutorial access (i.e. from settings)
        public void DoTutorial() => SceneLoader.Load(tutorialSceneIndex);
        void DeclineTutorial() => SceneLoader.Load(defaultSceneIndex);

        public void BeginPrompt()
        {
            if (!PlayerDidTutorial)
            {
                Dialog.Instance.YesNo(promptTitle, promptMessage, a => {
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
