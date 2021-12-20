using Core;
using TMPro;
using UnityEngine;

namespace UI.Visuals
{
    public class DialogueBox : Singleton<DialogueBox>
    {
        [SerializeField] TextMeshProUGUI nameText;
        [SerializeField, RequireSubstring("{0}")] string nameFormat;
        [SerializeField] TextBox textBox;

        public void Display(string speaker, params string[] text) => Display(false, speaker, text);

        public void Display(bool closable, string speaker, params string[] text)
        {
            if (string.IsNullOrEmpty(speaker)) nameText.gameObject.SetActive(false);
            else nameText.text = string.Format(nameFormat, speaker);
            
            textBox.Display(closable, text);
        }
    }
}
