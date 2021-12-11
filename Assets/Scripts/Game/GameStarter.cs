using Core;
using UI;
using UnityEngine;

namespace Game
{
    public class GameStarter : MonoBehaviour
    {
        [SerializeField] int tutorialSceneIndex;
        [SerializeField] int defaultSceneIndex;
        [SerializeField] int newGameSceneIndex;
        
        [Header("Tutorial Prompt")]
        [SerializeField] string promptTitle;
        [SerializeField, TextArea] string promptMessage;
        [SerializeField, TextArea] string promptNewVersionMessage;
        [SerializeField] Color promptColor;
        
        int destination;
        
        static bool PlayerDidLatestTutorial => PlayerPrefs.GetString(Key) == Application.version;
        static bool PlayerDidOldTutorial => PlayerPrefs.HasKey(Key) && PlayerPrefs.GetString(Key) != Application.version;

        const string Key = "TutorialDone";

        // Also use this for manual tutorial access (i.e. from settings)
        public void DoTutorial() => SceneLoader.Load(tutorialSceneIndex);
        void DeclineTutorial() => SceneLoader.Load(destination);
        
        public void StartNewGame()
        {
            destination = newGameSceneIndex;
            BeginPrompt();
        }
        
        public void ContinueGame()
        {
            destination = defaultSceneIndex;
            BeginPrompt();
        }

        void BeginPrompt()
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
            
            PlayerPrefs.SetString(Key, Application.version);
            PlayerPrefs.Save();
        }
    }
}
