using System.Collections;
using DG.Tweening;
using UnityEngine;
using Core;

namespace Upgrades
{
    public class Laptop : MonoBehaviour
    {
        [SerializeField] GameObject laptop;
        [SerializeField] Animator laptopAnimator;
        [SerializeField] string closeAnimation;
        [SerializeField] CanvasGroup uiGroup;
        [SerializeField] float fadeTime = 0.5f;

        int CloseAnimation => Animator.StringToHash(closeAnimation);

        Controls controls;
        bool animating;
        bool open;

        void Awake()
        {
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
            laptop.SetActive(true);
            yield return new WaitForSeconds(laptopAnimator.GetCurrentAnimatorStateInfo(0).length);
            ShowUI(true);
        }

        IEnumerator CloseLaptop()
        {
            ShowUI(false);
            laptopAnimator.Play(CloseAnimation);
            yield return new WaitForSeconds(laptopAnimator.GetCurrentAnimatorStateInfo(0).length);
            laptop.SetActive(false);
        }
    }
}
