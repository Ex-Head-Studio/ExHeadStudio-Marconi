using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System;
using System.Collections.Generic;

public class DisplayStatsClass
{

    //T objScript;
    //dati da aggiungere

    public DisplayStatsClass()
    {
       //this.objScript = objScript;
    }
    

}
public class DisplayStats : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    //Script per il display delle statistiche degli oggetti

    //tipi di classe a cui fa riferimento
    private Tile tileScript;
    private AShip shipScript;
    private AbstractObstacle obstacleScript;

    public static event Action<DisplayStatsClass> OnEntityHoverStarted;
    public static event Action<DisplayStatsClass> OnEntityHoverEnded;

    private void Start()
    {
        //controlli per prendere lo script corretto
        TryGetComponent<AShip>(out shipScript);
        TryGetComponent<Tile>(out tileScript);
        TryGetComponent<AbstractObstacle>(out obstacleScript);

        if (shipScript.faction == (int)Entity.ally)
        {

        }
    }


    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        if (shipScript != null)
        {
            OnEntityHoverStarted?.Invoke(new DisplayStatsClass());
        }
        else if (tileScript != null)
        {
            OnEntityHoverStarted?.Invoke(new DisplayStatsClass());
        }
        else if (obstacleScript != null)
        {
            OnEntityHoverStarted?.Invoke(new DisplayStatsClass());
        }
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        if (shipScript != null || tileScript != null || obstacleScript != null)
        {
            OnEntityHoverEnded?.Invoke(new DisplayStatsClass());
        }
    }


}
