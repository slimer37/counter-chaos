using System.Collections;
using DG.Tweening;
using UnityEngine;
using Core;
using Interactables.Holding;

namespace Upgrades
{
    public class Laptop : MonoBehaviour
    {
        [SerializeField] GameObject laptop;
        [SerializeField] CanvasGroup uiGroup;
        [SerializeField] Animator laptopAnimator;
        
        [Header("Animation Options")]
        [SerializeField] float fadeTime = 0.5f;
        [SerializeField] string openAnimation;
        [SerializeField] string closeAnimation;
        [SerializeField] float transitionDuration;

        int OpenAnimation => Animator.StringToHash(openAnimation);
        int CloseAnimation => Animator.StringToHash(closeAnimation);

        PlayerController controller;
        Controls controls;
        bool animating;
        bool open;

        void Awake()
        {
            controller = Player.Transform.GetComponent<PlayerController>();
            
            controls = new Controls();
            controls.Menu.OpenLaptop.performed += _ => OnOpenLaptop();
            controls.Enable();
            
            laptop.SetActive(false);
            uiGroup.gameObject.SetActive(true);
            uiGroup.alpha = 0;
            uiGroup.interactable = false;
        }

        void OnDestroy() => controls.Disable();

        void OnOpenLaptop()
        {
            if (!open && Inventory.Main.Holder.IsHoldingItem) return;
            
            open = !open;
            StopAllCoroutines();
            StartCoroutine(open ? OpenLaptop() : CloseLaptop());
        }

        void ShowUI(bool show)
        {
            uiGroup.interactable = show;
            uiGroup.DOFade(show ? 1 : 0, fadeTime);
            Cursor.lockState = show ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = show;
        }

        IEnumerator OpenLaptop()
        {
            controller.Suspend();
            laptop.SetActive(true);
            yield return PlayAnimation(OpenAnimation);
            ShowUI(true);
        }

        IEnumerator CloseLaptop()
        {
            controller.Suspend(false);
            ShowUI(false);
            yield return PlayAnimation(CloseAnimation);
            laptop.SetActive(false);
        }

        IEnumerator PlayAnimation(int id)
        {
            laptopAnimator.CrossFadeInFixedTime(id, transitionDuration);
            yield return new WaitForSeconds(laptopAnimator.GetCurrentAnimatorStateInfo(0).length);
        }
    }
}
