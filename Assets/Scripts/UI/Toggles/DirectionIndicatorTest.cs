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

    //uso uno unityevents, bisogna vedere se c'è qualcosa di migliore

    public static event Action<string> OnPointerEnterEvent;
    public static event Action<string> OnPointerExitEvent;
    private void Start()
    {
        toggleInformations = GetComponent<ToggleInformations>();
        shipName = toggleInformations.GetToggleSender();
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        OnPointerExitEvent?.Invoke(shipName);
        Debug.Log("Pointer Exit");
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        OnPointerEnterEvent?.Invoke(shipName);
        Debug.Log("Pointer Enter, ship: " + shipName);
    }




}
