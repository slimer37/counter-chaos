using System.Collections;
using DG.Tweening;
using UnityEngine;
using Interactables.Holding;

namespace Upgrades
{
    public class Laptop : MonoBehaviour
    {
        [SerializeField] InputProvider input;
        [SerializeField] GameObject laptop;
        [SerializeField] CanvasGroup uiGroup;
        [SerializeField] Animator laptopAnimator;
        [SerializeField] PlayerController controller;
        
        [Header("Animation Options")]
        [SerializeField] float fadeTime = 0.5f;
        [SerializeField] string openAnimation;
        [SerializeField] string closeAnimation;
        [SerializeField] float transitionDuration;

        int OpenAnimation => Animator.StringToHash(openAnimation);
        int CloseAnimation => Animator.StringToHash(closeAnimation);

        bool animating;
        bool open;

        void Awake()
        {
            input.OpenLaptop += OnOpenLaptop;
            
            laptop.SetActive(false);
            uiGroup.gameObject.SetActive(true);
            uiGroup.alpha = 0;
            uiGroup.interactable = uiGroup.blocksRaycasts = false;
        }

        void OnDestroy() => input.OpenLaptop -= OnOpenLaptop;

        void OnOpenLaptop()
        {
            if (!open && Inventory.Main.Holder.IsHoldingItem) return;
            
            open = !open;
            StopAllCoroutines();
            StartCoroutine(open ? OpenLaptop() : CloseLaptop());
        }

        void ShowUI(bool show)
        {
            uiGroup.interactable = uiGroup.blocksRaycasts = show;
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
