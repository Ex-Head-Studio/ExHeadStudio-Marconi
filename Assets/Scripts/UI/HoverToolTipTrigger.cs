using UnityEngine;
using UnityEngine.EventSystems;

public class HoverToolTipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RadioPanelPopup popup;

    public void OnPointerEnter(PointerEventData eventData)
    {
        popup.ShowPopup();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        popup.HidePopup();
    }
}
