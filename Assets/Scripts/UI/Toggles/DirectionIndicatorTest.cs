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
    private string shipName;
    private int directionIndicator;
    private int messageType;

    public static event Action<string, int, int> OnPointerEnterEvent;
    public static event Action<string> OnPointerExitEvent;
    private void Start()
    {
        toggleInformations = GetComponent<ToggleInformations>();
        shipName = toggleInformations.GetToggleSender();
        directionIndicator = toggleInformations.GetToggleDirection();
        messageType = toggleInformations.GetToggleMessageType();
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        OnPointerExitEvent?.Invoke(shipName);
       // Debug.Log("Pointer Exit");
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        OnPointerEnterEvent?.Invoke(shipName, directionIndicator, messageType);
       // Debug.Log("Pointer Enter, ship and direction: " + shipName + " " + directionIndicator); 
    }




}
