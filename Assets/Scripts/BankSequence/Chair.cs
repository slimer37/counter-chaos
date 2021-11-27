using UnityEngine;
using UnityEngine.InputSystem;

namespace BankSequence
{
    public class Chair : MonoBehaviour
    {
        [SerializeField] Camera cam;
        [SerializeField] Transform seat;
        [SerializeField, Min(0)] float sensitivity;
        [SerializeField, Min(0)] float yRotLimit = 85;
        [SerializeField, Min(0)] float xRotLimit = 85;
        [SerializeField, Min(0)] float seatRotLimit = 40;

        Vector3 camRot;
        Vector2 mouseDelta;
        
        void Awake()
        {
            sensitivity = PlayerPrefs.GetFloat("Sensitivity", sensitivity);
            camRot = cam.transform.localEulerAngles;
            
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        
        void OnMoveMouse(InputValue val) => mouseDelta = sensitivity * val.Get<Vector2>();

        void Update()
        {
            camRot.x = Mathf.Clamp(camRot.x - mouseDelta.y * Time.deltaTime, -yRotLimit, yRotLimit);
            camRot.y = Mathf.Clamp(camRot.y + mouseDelta.x * Time.deltaTime, -xRotLimit, xRotLimit);
            cam.transform.localEulerAngles = camRot;

            seat.localEulerAngles = Vector3.up * Mathf.Clamp(camRot.y, -seatRotLimit, seatRotLimit);
        }
    }
}
