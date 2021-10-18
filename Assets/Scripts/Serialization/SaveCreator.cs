using TMPro;
using Core;
using UnityEngine;
using UnityEngine.UI;

namespace Serialization
{
    public class SaveCreator : MonoBehaviour
    {
        [SerializeField] TMP_InputField saveNameField;
        [SerializeField] GameObject warningSpace;
        [SerializeField] TextMeshProUGUI warningText;
        [SerializeField] TMP_InputField playerNameField;
        [SerializeField] Button createButton;
        [SerializeField] int gameStartScene;

        void Awake()
        {
            saveNameField.onValueChanged.AddListener(ValidateFileName);
            createButton.onClick.AddListener(ApplyAndCreate);
        }
	
        void OnEnable()
        {
            saveNameField.text = playerNameField.text = "";
            warningSpace.SetActive(false);
            createButton.interactable = false;
        }

        void ValidateFileName(string fileName)
        {
            createButton.interactable = SaveSystem.NameIsValid(fileName);
            warningSpace.SetActive(!SaveSystem.NameIsValid(fileName) && !string.IsNullOrWhiteSpace(fileName));
            warningText.text = "";
            if (string.IsNullOrWhiteSpace(fileName)) return;
		
            if (!SaveSystem.NameIsValid(fileName))
            {
                fileName = SaveSystem.ToValidFileName(fileName);

                var isNewNameEmpty = string.IsNullOrWhiteSpace(fileName);
                createButton.interactable = !isNewNameEmpty;

                warningText.text = isNewNameEmpty
                    ? "No valid file name exists for this string. "
                    : $"Will be saved as: '{fileName + SaveSystem.SaveFileEnding}'. ";
            }

            if (SaveSystem.NameExists(fileName))
            {
                createButton.interactable = false;
                warningSpace.SetActive(true);
                warningText.text += $"'{fileName + SaveSystem.SaveFileEnding}' already exists.";
            }
        }

        void ApplyAndCreate()
        {
            var newData = new SaveData(saveNameField.text, playerNameField.text);
            newData.Save();
            SaveSystem.LoadedSave = newData;
            SceneLoader.Load(gameStartScene);
        }
    }
}
