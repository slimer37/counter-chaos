using System;
using System.IO;
using System.Reflection;

namespace Serialization
{
    public class SaveData
    {
        public float money;
        public string saveName;
        public string playerName;
        public string upgrades;
        public readonly DateTime creationDate;
	
        [ExcludeFromWrite] public readonly string baseFileName;

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
                if (fieldInfos[i].GetCustomAttribute<ExcludeFromWrite>() != null) continue;
                fieldInfos[i].SetValue(this, data[i]);
            }
        }

        public static implicit operator bool(SaveData data) => data != null;
    }
}