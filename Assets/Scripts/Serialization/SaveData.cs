using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace Serialization
{
    [JsonObject(MemberSerialization.Fields)]
    public class SaveData
    {
        public float money;
        public string saveName;
        public string playerName;
        public string upgrades;
        public DateTime creationDate;
        
        public string check;
	
        public readonly string baseFileName;

        public static readonly SaveData TemporarySave = new("Temporary", "Me", 0);
	
        public string AccessPath => Path.Combine(SaveSystem.SaveFolderLocation, FileName);
        public string FileName => baseFileName + SaveSystem.SaveFileEnding;
        public bool Compromised => !VerifyChecksum();

        public string GetJson() => JsonConvert.SerializeObject(this);

        public string GenerateChecksum()
        {
            // Generates checksum for the version of this object that has no checksum.
            var temp = check;
            check = "";
            using var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.ASCII.GetBytes(GetJson()));
            check = temp;
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        public bool VerifyChecksum() => check == GenerateChecksum();

        public SaveData(string saveName, string playerName, float startingMoney = 0)
        {
            if (string.IsNullOrWhiteSpace(saveName)) throw new Exception("Can't use blank or whitespace string as file name");
		
            this.saveName = saveName;
            this.playerName = playerName == "" ? "Nobody" : playerName;
            money = startingMoney;
            baseFileName = SaveSystem.ToValidFileName(saveName);
            creationDate = DateTime.Now;

            check = GenerateChecksum();
        }

        internal SaveData(string accessPath)
        {
            // Set reference file name on retrieval.
            var fileName = accessPath[(accessPath.LastIndexOf(Path.DirectorySeparatorChar) + 1)..];
            baseFileName = fileName[..^SaveSystem.SaveFileEnding.Length];
        }

        public static implicit operator bool(SaveData data) => data != null;
    }
}