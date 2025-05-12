using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro; 
using System;
public class DisplayStats : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    private ShipSO shipSO;
    public static event Action<ShipSO> OnShipOverStarted;
    public static event Action<ShipSO> OnShipOverEnded;

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        if(shipSO == null)
        {
            shipSO = GetComponent<AShip>().GetShipSO();
        }

        OnShipOverStarted?.Invoke(shipSO);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        OnShipOverEnded?.Invoke(shipSO);
    }


}
