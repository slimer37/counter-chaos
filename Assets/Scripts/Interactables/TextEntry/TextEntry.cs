using Interactables.Base;
using TMPro;
using UnityEngine;

namespace Interactables
{
    public class TextEntry : MonoBehaviour, IInteractHandler
    {
        [SerializeField] TextMeshPro text;
        [SerializeField, Min(1)] int charLimit = 1;

        bool enteringText;
        bool initGui;
        PlayerController tempController;

        void OnGUI()
        {
            if (!enteringText) return;

            var size = 500;

            var center = new Rect(size / 2, Screen.height / 2, Screen.width - size, 500);
            
            GUILayout.BeginArea(center);
            GUILayout.BeginVertical();

            var style = new GUIStyle(GUI.skin.box)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 50
            };
            var buttonStyle = new GUIStyle(GUI.skin.button) {fontSize = 50};
            
            GUI.SetNextControlName("text");
            text.text = GUILayout.TextArea(text.text, style, GUILayout.Height(100));
            GUI.FocusControl("text");

            if (!initGui)
            {
                initGui = true;
                var editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
                editor.selectIndex = text.text.Length;
            }

            if (text.text.Length > charLimit)
                text.text = text.text.Substring(0, charLimit);
            
            GUILayout.Space(10);
            
            GUILayout.BeginHorizontal();

            var difference = 400;
            
            GUILayout.Space(difference / 2);
            
            if (GUILayout.Button("Exit", buttonStyle, GUILayout.Width(center.width - difference), GUILayout.Height(50)))
                EndEntry();
            
            GUILayout.EndHorizontal();
            
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        public void OnInteract(Transform sender)
        {
            if (enteringText) return;
            enteringText = true;
            (tempController = sender.GetComponent<PlayerController>()).Suspend();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            initGui = false;
        }

        void EndEntry()
        {
            if (!enteringText) return;
            enteringText = false;
            tempController.Suspend(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
