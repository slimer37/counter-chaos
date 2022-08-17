using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace Serialization
{
    public static class SaveSystem
    {
        public static SaveData LoadedSave
        {
            get
            {
                if (!loadedSave)
                {
                    Debug.LogWarning("No save is loaded; using temporary save file.");
                    loadedSave = SaveData.TemporarySave;
                }
                return loadedSave;
            }
            set => loadedSave = value;
        }

        static SaveData loadedSave;
        
        const string SaveFolderName = "saves";
        public const string SaveFileEnding = ".store";

        static readonly string InvalidFileNameCharacters =
            new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

        public static readonly string SaveFolderLocation = Path.Combine(Application.persistentDataPath, SaveFolderName);
	
        [RuntimeInitializeOnLoadMethod]
        static void InitFolder()
        {
            if (!Directory.Exists(SaveFolderLocation))
                Directory.CreateDirectory(SaveFolderLocation);
        }

        public static string ToValidFileName(string checkedName) =>
            InvalidFileNameCharacters.Aggregate(checkedName, (current, c) => current.Replace(c.ToString(), "-"));
	
        public static bool NameIsValid(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return false;
            return !(from letter in fileName from invalidChar in InvalidFileNameCharacters where letter == invalidChar select letter).Any();
        }
	
        public static void Save(this SaveData saveData)
        {
            if (!saveData) throw new ArgumentNullException(nameof(saveData), "Got null data.");

            if (saveData == SaveData.TemporarySave)
            {
                Debug.LogWarning($"{nameof(Save)}() called on temporary save file. File will not be written.");
                return;
            }

            var json = JsonConvert.SerializeObject(saveData);
            File.WriteAllText(saveData.AccessPath, json);
        }
	
        public static bool LoadFromPath(string filePath, out SaveData retrievedData)
        {
            retrievedData = null;
            
            try
            {
                var jsonData = File.ReadAllText(filePath);
                retrievedData = SaveData.Blank(filePath);
                JsonConvert.PopulateObject(jsonData, retrievedData);
            }
            catch (Exception error)
            {
                Debug.LogError($"Exception loading at {filePath}\n{error}");
                return false;
            }

            return true;
        }

        public static bool NameExists(string match) => Directory.GetFiles(SaveFolderLocation)
            .Any(filePath => filePath.Substring(filePath.LastIndexOf(Path.DirectorySeparatorChar) + 1) == match + SaveFileEnding);
    }
}
