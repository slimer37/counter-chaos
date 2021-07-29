using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Products
{
    [CreateAssetMenu(menuName = "Game/Product", fileName = "New Product")]
    public class ProductInfo : ScriptableObject
    {
        [field: SerializeField] public string DisplayName { get; private set; }
        [field: SerializeField] public float Price { get; private set; }
        [field: SerializeField, TextArea] public string Description { get; private set; }
        
        public int ID { get; private set; }
        public string CompactName { get; private set; }
        public Texture2D Barcode { get; private set; }

        static readonly Dictionary<int, ProductInfo> IDTable = new Dictionary<int, ProductInfo>();
        
        public const int IDLength = 5;
        
        static readonly char[] Vowels = {'A', 'E', 'I', 'O', 'U'};

        public void Init(int seed)
        {
            var tempState = Random.state;
            Random.InitState(seed);
            
            string idString;
            do
            {
                idString = Random.Range(1, 10).ToString();
                for (var i = 0; i < IDLength - 1; i++)
                    idString += Random.Range(0, 10);
            } while (IDTable.ContainsKey(Convert.ToInt32(idString)));
            
            ID = Convert.ToInt32(idString);
            IDTable[ID] = this;
            Barcode = Products.Barcode.Generate();

            Random.state = tempState;

            CompactName = DisplayName.Replace(' ', '_').ToUpper();

            for (var i = 1; i < CompactName.Length - 1; i++)
            {
                // Delete vowels if they do not precede or succeed a space (replaced by underscores).
                if (Vowels.Contains(CompactName[i]) && CompactName[i - 1] != '_' && CompactName[i + 1] != '_')
                    CompactName = CompactName.Remove(i, 1);
            }
        }

        public static ProductInfo LookUp(int id) => IDTable.ContainsKey(id) ? IDTable[id] : null;
    }
}