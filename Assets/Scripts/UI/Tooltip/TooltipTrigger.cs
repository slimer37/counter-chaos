using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Tooltip
{
    public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public string titleText;
        public string descriptionText;

        public void OnPointerEnter(PointerEventData eventData) => TooltipDisplay.Instance.Show(titleText, descriptionText);
        public void OnPointerExit(PointerEventData eventData) => TooltipDisplay.Instance.Hide();
    }
}
