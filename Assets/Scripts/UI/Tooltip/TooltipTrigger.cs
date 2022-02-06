using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Tooltip
{
    public sealed class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public string TitleText
        {
            get => titleText;
            set
            {
                titleText = value;
                if (tooltipIsShowing) ShowTooltip(true);
            }
        }

        public string DescriptionText
        {
            get => descriptionText;
            set
            {
                descriptionText = value;
                if (tooltipIsShowing) ShowTooltip(true);
            }
        }
        
        [SerializeField] string titleText;
        [SerializeField, TextArea] string descriptionText;

        bool tooltipIsShowing;

        void ShowTooltip(bool show)
        {
            tooltipIsShowing = show;
            if (show) TooltipDisplay.Instance.Show(titleText, descriptionText);
            else TooltipDisplay.Instance.Hide();
        }

        public void OnPointerEnter(PointerEventData eventData) => ShowTooltip(true);
        public void OnPointerExit(PointerEventData eventData) => ShowTooltip(false);
    }
}
