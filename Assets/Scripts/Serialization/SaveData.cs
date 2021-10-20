using System;
using System.IO;

namespace Serialization
{
    public class SaveData
    {
        [Savable] public float money;
        [Savable] public string saveName;
        [Savable] public string playerName;
        [Savable] public string upgrades;
        [Savable] public readonly DateTime creationDate;
	
        public readonly string baseFileName;

        public static readonly SaveData TemporarySave = new SaveData("Temporary", "Me");
	
        public string AccessPath => Path.Combine(SaveSystem.SaveFolderLocation, FileName);
        public string FileName => baseFileName + SaveSystem.SaveFileEnding;
	
        public SaveData(string saveName, string playerName, float startingMoney = 0)
        {
            if (string.IsNullOrWhiteSpace(saveName)) throw new Exception("Can't use blank or whitespace string as file name");
		
            this.saveName = saveName;
            this.playerName = playerName == "" ? "Nobody" : playerName;
            money = startingMoney;
            baseFileName = SaveSystem.ToValidFileName(saveName);
            creationDate = DateTime.Now;
        }

        public SaveData(object[] data, string accessPath)
        {
            // Set reference file name on retrieval.
            var fileName = accessPath.Substring(accessPath.LastIndexOf(Path.DirectorySeparatorChar) + 1);
            baseFileName = fileName.Substring(0, fileName.Length - SaveSystem.SaveFileEnding.Length);
		
            var fieldInfos = typeof(SaveData).GetFields();
            for (var i = 0; i < fieldInfos.Length; i++)
            {
                if (!BinaryReaderWriter.IsSavable(fieldInfos[i])) continue;
                
                try
                { fieldInfos[i].SetValue(this, data[i]); }
                catch (Exception e)
                { throw new Exception($"While setting value {fieldInfos[i].Name}: {e.Message}"); }
            }
        }

        public static implicit operator bool(SaveData data) => data != null;
    }
}