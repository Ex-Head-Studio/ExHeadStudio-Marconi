using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class DirectionIndicatorTest : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Lo script viene associato ad un toggle, permette di vedere la direzione di movimento o attacco
    //della nave selezionata
    private ToggleInformations toggleInformations;
    private Toggle toggleScript;
    private string shipName;
    private int directionIndicator;
    private int messageType;

    public static event Action<string, int, int> OnPointerEnterEvent;
    public static event Action<string> OnPointerExitEvent;

    public static event Action<string, int, int> OnToggleSelectedEvent;
    public static event Action<string> OnToggleDeselectedEvent;
    private void Start()
    {
        toggleScript = GetComponent<Toggle>();  
        toggleInformations = GetComponent<ToggleInformations>();
        shipName = toggleInformations.GetToggleSender();
        directionIndicator = toggleInformations.GetToggleDirection();
        messageType = toggleInformations.GetToggleMessageType();
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        if(!toggleScript.isOn)
        {
            OnPointerExitEvent?.Invoke(shipName);
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        OnPointerEnterEvent?.Invoke(shipName, directionIndicator, messageType);
    }

    public void ShowArrowIndicator(bool toggleValue)
    {

        if(toggleValue)
        {
            OnToggleSelectedEvent?.Invoke(shipName, directionIndicator, messageType);
        }
        else
        {
            OnToggleDeselectedEvent?.Invoke(shipName);
        }

    }

    public void HideArrowIndicator()
    {
        OnToggleDeselectedEvent?.Invoke(shipName);
    }




}
