using UnityEngine;

namespace Prototyping
{
    internal class ColorMaterial : MonoBehaviour
    {
        [SerializeField] Color color = Color.white;

        void Awake() => GetComponent<Renderer>().material.color = color;
    }
}
