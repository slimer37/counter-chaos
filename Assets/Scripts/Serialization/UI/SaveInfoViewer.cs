using System.IO;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Serialization.UI
{
    public class SaveInfoViewer : MonoBehaviour
    {
        [SerializeField] SaveBrowser saveBrowser;
        [SerializeField] TextMeshProUGUI saveName;
        [SerializeField] TextMeshProUGUI playerName;
        [SerializeField] TextMeshProUGUI money;
        [SerializeField] TextMeshProUGUI date;
        [SerializeField] TextMeshProUGUI details;
        [SerializeField] GameObject noneSelectedText;
        [SerializeField] Button loadButton;
        [SerializeField] UnityEvent onStartGame;

        [Header("Deletion")]
        [SerializeField] Color deletionDialogColor;
        [SerializeField] Button deleteButton;

        SaveData focused;

        void Start()
        {
            loadButton.onClick.AddListener(LoadAndPlay);
            deleteButton.onClick.AddListener(BeginDelete);
        }

        void OnEnable() => EnableViewer(false);

        void LoadAndPlay()
        {
            SaveSystem.LoadedSave = focused;
            onStartGame?.Invoke();
        }
	
        public void View(SaveData save)
        {
            focused = save;
            saveName.text = save.saveName;
            date.text = "Created on " + save.creationDate.ToString("f");
            details.text = "Filename: " + save.FileName;
            playerName.text = save.playerName + "'s store:";
            money.text = save.money.ToString("c") + " owned";
            EnableViewer(true);
        }

        void EnableViewer(bool value)
        {
            if (!value)
            { saveName.text = playerName.text = money.text = date.text = details.text = ""; }
		
            loadButton.interactable = value;
            deleteButton.interactable = value;
            noneSelectedText.SetActive(!value);
        }

        void BeginDelete() => Dialog.Instance.YesNo("Save Deletion",
            $"Are you sure you want to delete\n'{focused.FileName}'?\nThis is permanent; not even the recycle bin can save it.",
            yes =>
            {
                if (yes) Delete();
            }, deletionDialogColor, false);

        void Delete()
        {
            File.Delete(Path.Combine(SaveSystem.SaveFolderLocation, focused.FileName));
            focused = null;
            EnableViewer(false);
            saveBrowser.Reload();
        }
    }
}
