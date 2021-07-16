using UnityEngine;

[CreateAssetMenu(menuName = "Game/Product", fileName = "New Product")]
public class ProductInfo : ScriptableObject
{
    [field: SerializeField] public string DisplayName { get; private set; }
    [field: SerializeField] public float Price { get; private set; }
    [field: SerializeField, TextArea] public string Description { get; private set; }
}