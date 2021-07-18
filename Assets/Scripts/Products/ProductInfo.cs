using System;
using System.Collections.Generic;
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
        public Texture2D Barcode { get; private set; }

        static List<int> usedIDs = new List<int>();
        const int IDLength = 5;

        public void Init(int barcodeSeed)
        {
            string idString;
            do
            {
                idString = Random.Range(1, 10).ToString();
                for (var i = 0; i < IDLength - 1; i++)
                    idString += Random.Range(0, 10);
            } while (usedIDs.Contains(Convert.ToInt32(idString)));
            
            ID = Convert.ToInt32(idString);
            usedIDs.Add(ID);
            Barcode = Products.Barcode.Generate(barcodeSeed);
        }
    }
}