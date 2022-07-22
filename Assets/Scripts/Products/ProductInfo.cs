using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Random = UnityEngine.Random;

namespace Products
{
    [CreateAssetMenu(menuName = "Game/Product", fileName = "New Product")]
    public class ProductInfo : ScriptableObject
    {
        [field: SerializeField] public string DisplayName { get; private set; }
        [field: SerializeField] public float Price { get; private set; }
        [field: SerializeField, TextArea] public string Description { get; private set; }
        [SerializeField] bool cannotBeScanned;
        [SerializeField] GameObject prefab;
        
        public GameObject Prefab => prefab;
        public int ID { get; private set; }
        public string CompactName { get; private set; }
        public Texture2D Barcode { get; private set; }
        public bool HasBarcode => Barcode;

        static readonly Dictionary<int, ProductInfo> IDTable = new();
        
        public const int IDLength = 5;
        
        static readonly char[] Vowels = {'A', 'E', 'I', 'O', 'U'};

        public GameObject Instantiate() => Instantiate(prefab);

        public Vector3 Size { get; private set; }

        internal void Init(int seed)
        {
            var tempState = Random.state;
            Random.InitState(seed);
            
            GenerateID();
            
            if (!cannotBeScanned) Barcode = Products.Barcode.Generate();
            
            Random.state = tempState;

            GenerateCompactName();

            Size = prefab.GetComponentInChildren<MeshFilter>().sharedMesh.bounds.size;
        }

        void GenerateID()
        {
            string idString;
            do
            {
                idString = Random.Range(1, 10).ToString();
                for (var i = 0; i < IDLength - 1; i++)
                    idString += Random.Range(0, 10);
            } while (IDTable.ContainsKey(Convert.ToInt32(idString)));
            
            ID = Convert.ToInt32(idString);
            IDTable[ID] = this;
        }

        void GenerateCompactName()
        {
            var rawName = DisplayName.Replace(' ', '_').ToUpper();
            CompactName = rawName[0].ToString();
            for (var i = 1; i < rawName.Length - 1; i++)
            {
                // Only add vowels if they precede or succeed a space, i.e. start or end a word.
                if (Vowels.Contains(rawName[i]) && rawName[i - 1] != '_' && rawName[i + 1] != '_') continue;

                CompactName += rawName[i];
            }
            CompactName += rawName[^1];
        }

        public static ProductInfo LookUp(int id) => IDTable.ContainsKey(id) ? IDTable[id] : null;

        public override string ToString() => $"{DisplayName} (#{ID})";
    }
}