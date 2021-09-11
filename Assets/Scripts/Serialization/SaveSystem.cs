using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Serialization
{
    public static class SaveSystem
    {
        public static SaveData loadedSave;
        
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

            using var writer = new BinaryWriter(File.Open(saveData.AccessPath, FileMode.Create));
            var readerWriter = new BinaryReaderWriter();
            readerWriter.Write(typeof(SaveData), saveData, writer);
            writer.Dispose();
        }
	
        public static bool LoadFromPath(string filePath, out SaveData retrievedData)
        {
            retrievedData = null;

            using var reader = new BinaryReader(File.Open(filePath, FileMode.Open));
            var readerWriter = new BinaryReaderWriter();
		
            try
            {
                var retrieved = readerWriter.Read(typeof(SaveData), reader);
                retrievedData = new SaveData(retrieved, filePath);
            }
            catch (Exception error)
            {
                Debug.LogError($"Exception loading at {filePath}\n{error}");
                return false;
            }
            finally { reader.Dispose(); }

            return true;
        }

        public static bool NameExists(string match) => Directory.GetFiles(SaveFolderLocation)
            .Any(filePath => filePath.Substring(filePath.LastIndexOf(Path.DirectorySeparatorChar) + 1) == match + SaveFileEnding);
    }
}
