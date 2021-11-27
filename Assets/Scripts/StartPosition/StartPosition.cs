using Core;
using UnityEngine;

public class StartPosition : MonoBehaviour
{
    [SerializeField] bool alsoCopyRotation;

    void Start()
    {
        var player = Player.Transform;
        player.position = transform.position;
        if (alsoCopyRotation) player.eulerAngles = Vector3.up * transform.eulerAngles.y;
    }
}
