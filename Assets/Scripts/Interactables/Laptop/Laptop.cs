using Core;
using DG.Tweening;
using Interactables.Base;
using UnityEngine;

namespace Interactables.Laptop
{
    public class Laptop : MonoBehaviour, IInteractHandler
    {
        [SerializeField] Hoverable hoverable;
        [SerializeField] Vector3 cameraPosition = Vector3.forward;
        [SerializeField] float animTime = 1;
        [SerializeField] CanvasGroup uiGroup;
        [SerializeField] float fadeTime = 0.5f;

        Controls controls;
        Vector3 tempCameraPos;
        Quaternion tempCameraRot;
        PlayerController tempController;

        bool animating;
        bool open;

        void Awake()
        {
            controls = new Controls();
            controls.Gameplay.SecondaryInteract.performed += _ => OnExit();
            uiGroup.gameObject.SetActive(true);
            uiGroup.alpha = 0;
            uiGroup.interactable = false;
            
            hoverable.OnAttemptHover += CanInteract;
        }
        
        bool CanInteract(Transform player)
        {
            if (open) return false;
            var inFront = Vector3.Dot(transform.forward, player.forward) < 0;
            enabled = inFront;
            return inFront;
        }

        void OnExit()
        {
            ShowUI(false);
            
            AnimateCamera(tempCameraPos, tempCameraRot, () => {
                controls.Disable();
                tempController.Suspend(false);
                open = false;
            });
        }

        public void OnInteract(Transform sender)
        {
            if (animating || !hoverable.enabled) return;

            open = true;

            (tempController = sender.GetComponent<PlayerController>()).Suspend();
            
            AnimateCamera(transform.TransformPoint(cameraPosition), Quaternion.LookRotation(-transform.forward),
                () => {
                    controls.Enable();
                    ShowUI(true);
                });
            
            var cam = Player.Camera.transform;
            tempCameraPos = cam.position;
            tempCameraRot = cam.rotation;
            
        }

        void AnimateCamera(Vector3 pos, Quaternion rot, TweenCallback onComplete)
        {
            animating = true;
            var cam = Player.Camera.transform;
            cam.DOMove(pos, animTime).OnComplete(onComplete);
            cam.DORotateQuaternion(rot, animTime).OnComplete(() => animating = false);
        }

        void ShowUI(bool show)
        {
            uiGroup.interactable = show;
            uiGroup.DOFade(show ? 1 : 0, fadeTime);
            Cursor.lockState = show ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = show;
        }
    }
}
