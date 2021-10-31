using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Core;

namespace Debugging
{
    public class DebugConsole : MonoBehaviour
    {
        [SerializeField] string cheatCode;
        [SerializeField] bool open;
        [SerializeField] GameObject enableWhenUnlocked;

        [Header("Text")]
        [SerializeField] Font outputFont;
        [SerializeField] Font inputFont;
        [SerializeField] int outputFontSize;
        [SerializeField] int inputFontSize;
        [SerializeField, ColorUsage(false)] Color errorColor;

        static DebugConsole current;

        static bool unlocked;
        int codeProgress;
        Controls controls;

        string input = "";
        string output = "<b>Wizards only, fools.</b>";
        Vector2 scrollPosition;

        void Awake()
        {
            if (current)
            {
                if (unlocked) enableWhenUnlocked.SetActive(true);
                Destroy(gameObject);
            }
            else
                current = this;

            var kb = Keyboard.current;
            kb.onTextInput += EnterCheatCode;

            controls = new Controls();
            controls.Enable();
            controls.Console.Open.performed += _ => open = unlocked ? !open : open;
            controls.Console.Submit.performed += _ => Submit();

            DontDestroyOnLoad(gameObject);
        }

        void Submit()
        {
            if (input == "") return;
	    
            if (output != "")
                Append("\n");

            if (input == "clear")
                output = "";
            else
            {
                try
                { Append(DebugCommands.Process(input)); }
                catch (System.Exception e)
                { Append($"<color=#{ColorUtility.ToHtmlStringRGB(errorColor)}>Error: {e.Message}</color>"); }
            }

            input = "";

            void Append(string text) => output = text + output;
        }

        void OnGUI()
        {
            GUI.skin.textArea.fontSize = outputFontSize;
            GUI.skin.textField.fontSize = inputFontSize;
            GUI.skin.textArea.font = outputFont;
            GUI.skin.textField.font = inputFont;
            GUI.skin.textArea.richText = true;

            if (!open) return;
        
            scrollPosition = GUILayout.BeginScrollView(scrollPosition,
                GUILayout.Width(Screen.width), GUILayout.MinHeight(200));
            GUILayout.TextArea(output);
            GUILayout.EndScrollView();
            
            input = GUILayout.TextField(input,
                GUILayout.Width(Screen.width - 5), GUILayout.MinHeight(20), GUILayout.ExpandHeight(true));
        }

        void EnterCheatCode(char letter)
        {
            if (unlocked || SceneManager.GetActiveScene().buildIndex != 0) return;
	    
            if (letter == cheatCode[codeProgress])
                codeProgress++;
            else
                codeProgress = 0;

            if (codeProgress == cheatCode.Length)
            {
                unlocked = true;
                if (enableWhenUnlocked) enableWhenUnlocked.SetActive(true);
            }
        }
    }
}
