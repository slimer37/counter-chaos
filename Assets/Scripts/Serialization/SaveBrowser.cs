using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace Serialization
{
    public class SaveBrowser : MonoBehaviour
    {
        [SerializeField] GameObject saveDisplayBoxPrefab;
        [SerializeField] GameObject noFilesText;
        [SerializeField] GameObject errorText;
        [SerializeField] Transform boxParent;
        [SerializeField] SaveInfoViewer saveInfoViewer;

        void OnEnable() => Reload();
	
        public void Reload()
        {
            var saveFiles = Directory.GetFiles(SaveSystem.SaveFolderLocation);

            noFilesText.SetActive(saveFiles.Length == 0);
            errorText.SetActive(false);

            foreach (Transform child in boxParent)
                Destroy(child.gameObject);

            if (saveFiles.Length == 0) return;
		
            foreach (var savePath in saveFiles)
            {
                if (!SaveSystem.LoadFromPath(savePath, out var data))
                {
                    errorText.SetActive(true);
                    return;
                }
			
                var cloneBox = Instantiate(saveDisplayBoxPrefab, boxParent).GetComponent<SaveDisplayBox>();
                cloneBox.DisplayFor(data);
                cloneBox.AttachToViewer(saveInfoViewer);
                cloneBox.gameObject.SetActive(true);
            }
        }

        public void OpenSaveFolder() => Process.Start(SaveSystem.SaveFolderLocation.TrimEnd('\\', '/'));
    }
}
