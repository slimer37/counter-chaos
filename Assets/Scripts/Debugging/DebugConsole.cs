using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Debugging
{
    public class DebugConsole : MonoBehaviour
    {
        [SerializeField] string cheatCode;
        [SerializeField] bool open;
        [SerializeField] GameObject enableWhenUnlocked;
        [SerializeField] InputProvider inputProvider;

        [Header("Text")]
        [SerializeField] Font outputFont;
        [SerializeField] Font inputFont;
        [SerializeField] int outputFontSize;
        [SerializeField] int inputFontSize;
        [SerializeField, ColorUsage(false)] Color errorColor;

        static bool unlocked;
        int codeProgress;

        string input = "";
        string output = "<b>Wizards only, fools.</b>";
        Vector2 scrollPosition;

        void Awake()
        {
            var kb = Keyboard.current;
            kb.onTextInput += EnterCheatCode;

            inputProvider.ConsoleOpen += () => open = unlocked ? !open : open;
            inputProvider.ConsoleSubmit += Submit;

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
