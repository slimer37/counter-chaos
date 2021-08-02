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
        Transform mainCamera;
        Vector3 tempCameraPos;
        Quaternion tempCameraRot;
        GameObject tempPlayer;

        bool animating;

        void Awake()
        {
            controls = new Controls();
            controls.Gameplay.SecondaryInteract.performed += _ => OnExit();
            mainCamera = Camera.main.transform;
            uiGroup.gameObject.SetActive(true);
            uiGroup.alpha = 0;
            uiGroup.interactable = false;
            
            hoverable.OnAttemptHover = CheckPlayerFacing;
        }
        
        bool CheckPlayerFacing(Transform player)
        {
            var inFront = Vector3.Dot(transform.forward, player.forward) < 0;
            enabled = inFront;
            return inFront;
        }

        void OnExit()
        {
            ShowUI(false);
            
            AnimateCamera(tempCameraPos, tempCameraRot, () => {
                controls.Disable();
                tempPlayer.SendMessage("EnableController", true);
                hoverable.enabled = true;
            });
        }

        public void OnInteract(Transform sender)
        {
            if (animating || !hoverable.enabled) return;

            hoverable.enabled = false;

            tempPlayer = sender.gameObject;
            tempPlayer.SendMessage("EnableController", false);
            
            AnimateCamera(transform.TransformPoint(cameraPosition), Quaternion.LookRotation(-transform.forward),
                () => {
                    controls.Enable();
                    ShowUI(true);
                });
            tempCameraPos = mainCamera.position;
            tempCameraRot = mainCamera.rotation;
            
        }

        void AnimateCamera(Vector3 pos, Quaternion rot, TweenCallback onComplete)
        {
            animating = true;
            mainCamera.DOMove(pos, animTime).OnComplete(onComplete);
            mainCamera.DORotateQuaternion(rot, animTime).OnComplete(() => animating = false);
        }

        void ShowUI(bool show)
        {
            uiGroup.interactable = show;
            uiGroup.DOFade(show ? 1 : 0, fadeTime);
        }
    }
}
